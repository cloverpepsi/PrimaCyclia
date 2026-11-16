using MonoMod.Utils;
using Quintessential;
using System.Collections.Generic;

namespace PrimaCyclia;

internal class SublimationGlyph {
    public static Texture sublimation_base = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/sublimation/base");
    public static Texture[][] sublimation_iris_anim = [
        PrimaCyclia.GetAnimation("textures/prima_cyclia/parts/sublimation/iris_1_full.array","iris",16),
        PrimaCyclia.GetAnimation("textures/prima_cyclia/parts/sublimation/iris_2_full.array","iris",16),
        PrimaCyclia.GetAnimation("textures/prima_cyclia/parts/sublimation/iris_3_full.array","iris",16),
        PrimaCyclia.GetAnimation("textures/prima_cyclia/parts/sublimation/iris_4_full.array","iris",16),
        PrimaCyclia.GetAnimation("textures/prima_cyclia/parts/sublimation/iris_5_full.array","iris",16),
        PrimaCyclia.GetAnimation("textures/prima_cyclia/parts/sublimation/iris_6_full.array","iris",16),
    ];
    public static readonly HexIndex[] sublimationIrises = [new(-1, 2), new(1, 1), new(2, -1), new(1, -2), new(-1, -1), new(-2, 1)];

    enum IrisMode { open, closed, opening, closing }
    private class SublimeState {
        public IrisMode mode = IrisMode.open;
        public int cycle_modified = 0;

        public SublimeState(IrisMode m, int cycle) {
            mode = m;
            cycle_modified = cycle;
        }
    }
    public static void Render(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {

        PartSimState pss = editor.GetSimulation().GetSimState(part);

        IntermediatePartState class_424 = editor.GetIntermState(part, pos);
        PartRenderer class_425 = new(class_424.pos, class_424.rotation, Editor.GetLightSourcePosition());

        Texture tex_to_render = sublimation_base;
        renderer.RenderBase(tex_to_render, tex_to_render.GetHalfSizeRounded() + new Vector2(1f, 1f));
        renderer.RenderUpright(Assets.textures.parts.disposal.lower_ring, new HexIndex(0, 0), new Vector2(1f, 1f));
        renderer.RenderUpright(Assets.textures.parts.disposal.lower_fixed, new HexIndex(0, 0), new Vector2(1f, 1f));
        renderer.RenderUpright(Assets.textures.parts.disposal.upper_fixed, new HexIndex(0, 0), new Vector2(1f, 1f));
        renderer.RenderUpright(Assets.textures.parts.disposal.upper_rings, new HexIndex(0, 0), new Vector2(1f, 1f));
        Texture[] field_17 = Assets.textures.parts.disposal_gears;
        int num13 = 0;
        if (editor.GetSimPlayState() != SimPlayState.Stopped) {
            num13 = (int)(Time.NowInSeconds() * 30f) % field_17.Length;
        }
        TextureRenderer.Render(field_17[num13], class_425.partPos + new Vector2(-62f, -62f));

        DynamicData dyn_pss = new(pss);
        object stateOb = dyn_pss.Get("sublimation_state");
        SublimeState is_open = new(IrisMode.open, -1);

        if (stateOb is not null) {
            is_open = (SublimeState)stateOb;
        } else { dyn_pss.Set("sublimation_state", is_open); }

        IntermediatePartState uco = editor.GetIntermState(part, pos);
        float time = editor.GetCycleTime();

        int irisFrame = 15;

        if (is_open.mode == IrisMode.opening) {
            irisFrame = Utils.Clamp((int)(Utils.InterpolateLinear(1f, -1f, time) * 16f), 0, 15);
            if (time > 0.5f) { irisFrame = 0; }
        }
        if (is_open.mode == IrisMode.closing) {
            irisFrame = Utils.Clamp((int)(Utils.InterpolateLinear(1f, -1f, 1 - time) * 16f), 0, 15);
        }
        if (is_open.mode == IrisMode.open) { irisFrame = 0; }
        if (is_open.mode == IrisMode.closed) { irisFrame = 15; }

        renderer.RenderUpright(Assets.textures.parts.iris_full[irisFrame], Parts.center, Vector2.Zero);

        for (int i = 0; i < 6; i++) {

            int outputIrisFrame = 15;
            bool afterIrisOpens = false;

            HexIndex h = sublimationIrises[i];
            Vector2 location = uco.pos + HexGrid.standardGrid.ToPixelCoords(h).Rotated(uco.rotation);

            bool renderRisingAtom = pss.processingAtoms is not null && pss.processingAtoms.Length > i;

            Molecule risingAtom = Molecule.CreateMonoatomic(renderRisingAtom ? pss.processingAtoms[i] : Atoms.Sulfur);

            renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, h, 3f);

            if (pss.isProcessing && renderRisingAtom) {
                outputIrisFrame = Utils.Clamp((int)(Utils.InterpolateLinear(1f, -1f, time) * 16f), 0, 15);
                afterIrisOpens = time > 0.5f;
                if (!afterIrisOpens) {
                    // show atom rising behind iris
                    Editor.RenderMolecule(risingAtom, location, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
                }
            }
            renderer.RenderUpright(sublimation_iris_anim[i][outputIrisFrame], h, Vector2.Zero);
            renderer.RenderRotating(Assets.textures.parts.animismus.output_above_iris, h, Vector2.Zero);
            if (pss.isProcessing && afterIrisOpens) {
                // show atom rising infront of iris
                Editor.RenderMolecule(risingAtom, location, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
            }
        }
    }

    public static bool GetAtomWeight(AtomType atomType, out int weight) {
        weight = 0;
        return AtomTag.AtomTags["prima_cyclia:$sublimation_weight"].HasAtom(atomType, out var weightKey) &&
            weightKey.Value.namespc == "" && int.TryParse(weightKey.Value.name.ToString(), out weight);
    }

    public static bool OnCycle(Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) {
        DynamicData dyn_pss = new(simState);
        object stateOb = dyn_pss.Get("sublimation_state");
        SublimeState is_open = new(IrisMode.open, -1);

        if (stateOb is not null) {
            is_open = (SublimeState)stateOb;
        } else { dyn_pss.Set("sublimation_state", is_open); }

        bool hexes_covered = false;
        foreach (HexIndex h in sublimationIrises)
            if (sim.GetAtomReference(part, h, false, out AtomReference a)) { hexes_covered = true; }
        if (sim.GetAtomReference(part, Parts.center, false, out AtomReference positioned) && !GetAtomWeight(positioned.atomType, out _)) { hexes_covered = true; }

        int current_cycle = sim.GetCycle();
        if (is_open.cycle_modified < current_cycle && isCycleStart) {
            if (hexes_covered && is_open.mode == IrisMode.closing) { is_open.mode = IrisMode.closed; is_open.cycle_modified = current_cycle; }
            if (hexes_covered && is_open.mode != IrisMode.closed) { is_open.mode = IrisMode.closing; is_open.cycle_modified = current_cycle; }
            if (!hexes_covered && is_open.mode == IrisMode.opening) { is_open.mode = IrisMode.open; is_open.cycle_modified = current_cycle; }
            if (!hexes_covered && is_open.mode != IrisMode.open) { is_open.mode = IrisMode.opening; is_open.cycle_modified = current_cycle; }
        }

        bool wasActivated = false;
        if (is_open.mode == IrisMode.open && isCycleStart && !simState.isProcessing) {
            if (sim.GetAtomReference(part, Parts.center, false, out AtomReference input) && !input.inMultiAtomMolecule && !input.isHeldByArm) {
                input.molecule.RemoveAtom(input.pos);
                sim.solutionEditor.consumptionEffects.Add(new ConsumptionEffect(sim.solutionEditor, input));
                int weight = 1;
                if (GetAtomWeight(input.atomType, out int value)) { weight = value; }
                Parts.SublimationSound.PlaySound(sim.solutionEditor.GetVolumeMultiplier());

                if (weight > 0) {
                    is_open.mode = IrisMode.closing; is_open.cycle_modified = current_cycle;
                    simState.processingAtoms = new AtomType[weight];
                    for (int i = 0; i < weight; i++) {
                        HexIndex h = sublimationIrises[i];
                        sim.additionalCollisions.Add(new(Sim.ColliderType.Atom, HexGrid.standardGrid.ToPixelCoords(part.GlobalPosFromLocal(h))) { radius = 15f });
                        simState.processingAtoms[i] = Atoms.Sulfur;
                    }
                    Parts.SublimationProduction[(weight - 1) / 2].PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                    simState.isProcessing = true;
                }
                wasActivated = true;
            }
        } else if (simState.isProcessing) {
            for (int i = 0; i < simState.processingAtoms.Length; i++) {
                HexIndex h = sublimationIrises[i];
                Molecule m = new();
                m.AddAtom(new Atom(simState.processingAtoms[i]), part.GlobalPosFromLocal(h));
                sim.molecules.Add(m);
            }
            wasActivated = true;
        }
        dyn_pss.Set("sublimation_state", is_open);
        return wasActivated;
    }
}
