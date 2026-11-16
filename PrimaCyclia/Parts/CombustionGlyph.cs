using Quintessential;

namespace PrimaCyclia;

internal class CombustionGlyph {
    public static void Render(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {
        Vector2 offset = new(41f, 120f);

        renderer.RenderBase(Assets.textures.parts.animismus.@base, Vector2.Zero, offset, 0f);

        foreach (HexIndex h in Parts.Combustion.glyphHexes) { renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, h, 3f); }

        renderer.RenderRotating(Assets.textures.parts.projection_glyph.quicksilver_input, Parts.center, Vector2.Zero); //input bowl 1
        renderer.RenderRotating(Assets.textures.parts.projection_glyph.quicksilver_input, Parts.hex_right, Vector2.Zero); //input bowl 1
        renderer.RenderUpright(Parts.volatiles_input, Parts.center, Vector2.Zero);
        renderer.RenderUpright(Parts.volatiles_input, Parts.hex_right, Vector2.Zero);

        PartSimState pss = editor.GetSimulation().GetSimState(part);
        IntermediatePartState uco = editor.GetIntermState(part, pos);
        float time = editor.GetCycleTime();

        int irisFrame = 15;
        bool afterIrisOpens = false;
        Molecule risingAtom_1 = pss.isProcessing ? Molecule.CreateMonoatomic(pss.processingAtoms[0]) : Molecule.CreateMonoatomic(Parts.primeAtoms[2]);
        Molecule risingAtom_2 = pss.isProcessing ? Molecule.CreateMonoatomic(pss.processingAtoms[1]) : Molecule.CreateMonoatomic(Parts.primeAtoms[2]);

        Vector2 risingOffset_1 = uco.pos + HexGrid.standardGrid.ToPixelCoords(Parts.hex_down_right).Rotated(uco.rotation);
        Vector2 risingOffset_2 = uco.pos + HexGrid.standardGrid.ToPixelCoords(Parts.hex_up_right).Rotated(uco.rotation);

        if (pss.isProcessing) {
            irisFrame = Utils.Clamp((int)(Utils.InterpolateLinear(1f, -1f, time) * 16f), 0, 15);
            afterIrisOpens = time > 0.5f;
            if (!afterIrisOpens) {
                // show atom rising behind iris
                Editor.RenderMolecule(risingAtom_1, risingOffset_1, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
                Editor.RenderMolecule(risingAtom_2, risingOffset_2, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
            }
        }
        renderer.RenderUpright(Parts.phlog_iris[irisFrame], Parts.hex_down_right, Vector2.Zero);
        renderer.RenderUpright(Parts.oxygen_iris[irisFrame], Parts.hex_up_right, Vector2.Zero);
        renderer.RenderRotating(Assets.textures.parts.animismus.output_above_iris, Parts.hex_down_right, Vector2.Zero);
        renderer.RenderRotating(Assets.textures.parts.animismus.output_above_iris, Parts.hex_up_right, Vector2.Zero);
        if (pss.isProcessing && afterIrisOpens) {
            // show atom rising infront of iris
            Editor.RenderMolecule(risingAtom_1, risingOffset_1, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
            Editor.RenderMolecule(risingAtom_2, risingOffset_2, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
        }
    }

    public static bool OnCycle(Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) {
        if (isCycleStart && !simState.isProcessing) {
            if (sim.GetAtomReference(part, Parts.center, false, out AtomReference atom_1) && !atom_1.inMultiAtomMolecule && !atom_1.isHeldByArm &&
                sim.GetAtomReference(part, Parts.hex_right, false, out AtomReference atom_2) && !atom_2.inMultiAtomMolecule && !atom_2.isHeldByArm && (
                    (atom_1.atomType == Atoms.Sulfur && atom_2.atomType == Atoms.Sulfur) ||
                    (atom_1.atomType != atom_2.atomType && Parts.volatileAtoms.Contains(atom_1.atomType) && Parts.volatileAtoms.Contains(atom_2.atomType))
                )) {
                if (!sim.HasAtomAt(part, Parts.hex_down_right, true) && !sim.HasAtomAt(part, Parts.hex_up_right, true)) {

                    AtomType[] produced_atoms;
                    if (atom_1.atomType == atom_2.atomType) {
                        produced_atoms = [Atoms.Phlogiston, Atoms.Oxygen];
                    } else {
                        AtomType to_create = Atoms.Sulfur;
                        foreach (AtomType a in Parts.volatileAtoms) {
                            if (a != atom_1.atomType && a != atom_2.atomType) {
                                to_create = a;
                            }
                        }
                        produced_atoms = [to_create, to_create];
                    }

                    simState.processingAtoms = produced_atoms;

                    atom_1.molecule.RemoveAtom(atom_1.pos);
                    atom_2.molecule.RemoveAtom(atom_2.pos);
                    sim.solutionEditor.consumptionEffects.Add(new ConsumptionEffect(sim.solutionEditor, atom_1));
                    sim.solutionEditor.consumptionEffects.Add(new ConsumptionEffect(sim.solutionEditor, atom_2));
                    simState.isProcessing = true;
                    sim.additionalCollisions.Add(new(Sim.ColliderType.Atom, HexGrid.standardGrid.ToPixelCoords(part.GlobalPosFromLocal(Parts.hex_down_right))) { radius = 15f });
                    sim.additionalCollisions.Add(new(Sim.ColliderType.Atom, HexGrid.standardGrid.ToPixelCoords(part.GlobalPosFromLocal(Parts.hex_up_right))) { radius = 15f });

                    if (produced_atoms[0] == produced_atoms[1]) {
                        if (produced_atoms[0] == Atoms.Phlogiston) {
                            Parts.CombustionPhlogRelease.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                        } else if (produced_atoms[0] == Atoms.Phlogiston) {
                            Parts.CombustionOxygenRelease.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                        } else {
                            Parts.CombustionBothRelease.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                        }
                    } else {
                        Parts.CombustionBothRelease.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                    }
                    return true;
                }
            }
        } else if (simState.isProcessing) {
            Molecule m = new();
            m.AddAtom(new Atom(simState.processingAtoms[0]), part.GlobalPosFromLocal(Parts.hex_down_right));
            sim.molecules.Add(m);
            m = new();
            m.AddAtom(new Atom(simState.processingAtoms[1]), part.GlobalPosFromLocal(Parts.hex_up_right));
            sim.molecules.Add(m);
            return true;
        }
        return false;
    }
}
