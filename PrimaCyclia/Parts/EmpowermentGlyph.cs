using MonoMod.Utils;
using Quintessential;
using System.Collections.Generic;
using System.Linq;

namespace PrimaCyclia;

internal class EmpowermentGlyph {
    public static Texture empowerment_base = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/empowerment/base");
    public static Texture empowerment_mesh = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/empowerment/mesh");
    public static Texture empowerment_overlay = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/empowerment/overlay");
    public static Texture[] empowerment_lights = [
        AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/empowerment/b2_light"),
        AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/empowerment/b3_light"),
        AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/empowerment/b1_light")
    ];

    private class EmpowerState {
        public bool[] canBond = [false, false, false];
        public bool containsCharge = false;
        public EmpowerState(bool[] bondables) { canBond = bondables; }
        public bool IsEmpty() { return Enumerable.SequenceEqual(canBond, [false, false, false]); }
    }

    public static void Render(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {
        PartSimState pss = editor.GetSimulation().GetSimState(part);
        IntermediatePartState uco = editor.GetIntermState(part, pos);
        float time = editor.GetCycleTime();

        bool renderMesh = true;
        foreach (Part other in editor.GetSolution().parts) {
            if (other.GetType() == PartTypes.triplexBonder) {
                if (IsEmpowered(part, other)) {
                    renderMesh = false;
                }
            }
        }

        if (renderMesh) {
            renderer.RenderBase(empowerment_mesh, empowerment_mesh.GetHalfSizeRounded() + new Vector2(1f, 1f));
        }

        renderer.RenderBase(empowerment_base, empowerment_base.GetHalfSizeRounded() + new Vector2(1f, 1f));
        renderer.RenderBase(empowerment_overlay, empowerment_overlay.GetHalfSizeRounded() + new Vector2(1f, 1f));

        DynamicData dyn_pss = new(pss);
        object stateOb = dyn_pss.Get("empowerment_state");
        EmpowerState emp = new([false, false, false]);

        if (stateOb is not null) {
            emp = (EmpowerState)stateOb;
        } else { dyn_pss.Set("empowerment_state", emp); }

        for (int i = 0; i < 3; i++) {
            if (emp.canBond[i]) { renderer.RenderBase(empowerment_lights[i], empowerment_lights[i].GetHalfSizeRounded() + new Vector2(1f, 1f)); }
        }

        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, Parts.hex_up_right, 3f);
        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, Parts.hex_up_left, 3f);

        renderer.RenderRotating(Assets.textures.parts.projection_glyph.quicksilver_input, Parts.hex_up_left, Vector2.Zero); //input bowl 1

        renderer.RenderUpright(Parts.oxygen_input, Parts.hex_up_left, Vector2.Zero);

        int irisFrame = 15;
        bool afterIrisOpens = false;
        Molecule risingAtom = Molecule.CreateMonoatomic(Atoms.Phlogiston);

        Vector2 risingOffset = uco.pos + HexGrid.standardGrid.ToPixelCoords(Parts.hex_up_right).Rotated(uco.rotation);

        if (pss.isProcessing) {
            irisFrame = Utils.Clamp((int)(Utils.InterpolateLinear(1f, -1f, time) * 16f), 0, 15);
            afterIrisOpens = time > 0.5f;
            if (!afterIrisOpens) {
                // show atom rising behind iris
                Editor.RenderMolecule(risingAtom, risingOffset, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
            }
        }
        renderer.RenderUpright(Parts.phlog_iris[irisFrame], Parts.hex_up_right, Vector2.Zero);
        renderer.RenderRotating(Assets.textures.parts.animismus.output_above_iris, Parts.hex_up_right, Vector2.Zero);
        if (pss.isProcessing && afterIrisOpens) {
            // show atom rising infront of iris
            Editor.RenderMolecule(risingAtom, risingOffset, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
        }
    }

    public static bool OnCycle(Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) {
        DynamicData dyn_pss = new(simState);
        object stateOb = dyn_pss.Get("empowerment_state");
        EmpowerState emp = new([false, false, false]);

        if (stateOb is not null) {
            emp = (EmpowerState)stateOb;
        } else { dyn_pss.Set("empowerment_state", emp); }

        if (isCycleStart && !simState.isProcessing && emp.IsEmpty()) {
            if (!emp.containsCharge) {
                if (sim.GetAtomReference(part, Parts.hex_up_left, false, out AtomReference atom_1) && !atom_1.inMultiAtomMolecule && !atom_1.isHeldByArm && atom_1.atomType == Atoms.Oxygen) {
                    emp.canBond = [true, true, true];
                    emp.containsCharge = true;
                    atom_1.molecule.RemoveAtom(atom_1.pos);
                    sim.solutionEditor.consumptionEffects.Add(new ConsumptionEffect(sim.solutionEditor, atom_1));
                    Parts.EmpowermentConsume.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                }
            } else {
                emp.containsCharge = false;
                simState.isProcessing = true;
                Parts.EmpowermentProduce.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                sim.additionalCollisions.Add(new(Sim.ColliderType.Atom, HexGrid.standardGrid.ToPixelCoords(part.GlobalPosFromLocal(Parts.hex_up_right))) { radius = 15f });
            }
        } else if (simState.isProcessing) {
            Molecule m = new();
            m.AddAtom(new Atom(Atoms.Phlogiston), part.GlobalPosFromLocal(Parts.hex_up_right));
            sim.molecules.Add(m);
        }

        foreach (Part other in sim.solutionEditor.GetSolution().parts) {
            if (other.GetType() == PartTypes.triplexBonder) {
                if (IsEmpowered(part, other)) {
                    BonderInfo[] bonder_set = PartTypes.triplexBonder.bonders;
                    for (int index = 0; index < bonder_set.Length; index++) {
                        BonderInfo bonder = bonder_set[index];
                        if (!sim.GetAtomReference(other, bonder.hexPos1, false, out AtomReference leftAtom) || !sim.GetAtomReference(other, bonder.hexPos2, false, out AtomReference rightAtom)) {
                            continue;
                        }
                        if (emp.canBond[index]) {
                            if (leftAtom.molecule != rightAtom.molecule) {
                                sim.molecules.Remove(leftAtom.molecule);
                                sim.molecules.Remove(rightAtom.molecule);
                                var joined = leftAtom.molecule.MergeWith(rightAtom.molecule);
                                sim.molecules.Add(joined);
                            }
                            HexIndex h1 = other.GlobalPosFromLocal(bonder.hexPos1);
                            HexIndex h2 = other.GlobalPosFromLocal(bonder.hexPos2);
                            if (sim.FindAtom(h1).GetOrDefault(out AtomReference atom1) && sim.FindAtom(h2).GetOrDefault(out AtomReference atom2)) {
                                //PrimaCyclia.Logger.Critical("Found - 1");
                                Molecule subject = atom1.molecule;
                                if (subject == atom2.molecule) {
                                    //PrimaCyclia.Logger.Critical("Found - 2");
                                    bool wasAnyAdded = false;
                                    foreach (BondType bondType in bonder.bondTypes) {
                                        BondTexture bondTexture = bondType.bondTexture;
                                        BondEffect bondEffect = new(sim.solutionEditor, bondTexture.bondAnim, 60f, bondTexture.textureOffset);
                                        if (subject.AddBond(bondType, h1, h2, bondEffect)) {
                                            Vector2 vector2 = Utils.InterpolateVect(HexGrid.standardGrid.ToPixelCoords(h1), HexGrid.standardGrid.ToPixelCoords(h2), 0.5f);
                                            sim.solutionEditor.glyphEffects.Add(new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime, vector2, bondTexture.glyphAnim, 30f, Vector2.Zero, HexGrid.standardGrid.ToPixelCoords(h2 - h1).Angle(), 0f));
                                            //sim.PlaySound(bondTexture.bondSound);
                                            wasAnyAdded = true;
                                        }
                                    }
                                    if (wasAnyAdded) {
                                        //PrimaCyclia.Logger.Critical("Found - 3");
                                        Parts.EmpowermentBonds[index].PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                                        emp.canBond[index] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        dyn_pss.Set("empowerment_state", emp);
        return true;
    }


    public static bool IsEmpowered(Part empowerment, Part prisma) {
        HashSet<HexIndex> checked_hexes = [empowerment.GlobalPosFromLocal(Parts.center), empowerment.GlobalPosFromLocal(Parts.hex_down_left), empowerment.GlobalPosFromLocal(Parts.hex_down_right)];
        HashSet<HexIndex> my_triplex = [prisma.GlobalPosFromLocal(Parts.center), prisma.GlobalPosFromLocal(Parts.hex_right), prisma.GlobalPosFromLocal(Parts.hex_up_right)];
        return checked_hexes.SetEquals(my_triplex);
    }
}
