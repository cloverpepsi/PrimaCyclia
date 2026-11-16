using MonoMod.Utils;
using Quintessential;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrimaCyclia;

public static class Parts {
    public static PartType Polarity, Neutrality, Cyclicity, Primality, Empowerment, Combustion, Sublimation, Tolomeo;
    const float sixtyDegrees = 60f * (float)Math.PI / 180f;
    public static readonly HexIndex center = new(0, 0);
    public static readonly HexIndex hex_right = new(1,0);
    public static readonly HexIndex hex_left = new(-1,0);
    public static readonly HexIndex hex_up_right = new(0,1);
    public static readonly HexIndex hex_down_right = new(1,-1);
    public static readonly HexIndex hex_up_left = new(-1,1);
    public static readonly HexIndex hex_down_left = new(0,-1);
    static HexRotation[] HexArmRotations => PartTypes.hexArm.armRotations;
    const string TolomeoWheelAtomsField = "prima_cyclia_TolomeoMolecule";
    public static Vector2 hexGraphicalOffset(HexIndex hex) => HexGrid.standardGrid.ToPixelCoords(hex);
    static LightDirTextures atomCageLighting => Assets.textures.parts.atom_cage_lighting;

    // custom glyph textures
    public static Texture prime_symbols = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/prime_symbols");

    public static Texture oxygen_input = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/oxygen_input");
    public static Texture phlog_input = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/phlogiston_input");
    public static Texture sulfur_input = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/sulfur_input");
    public static Texture primes_input = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/primes_input");
    public static Texture neutrality_input = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/qs_sulfur_input");
    public static Texture volatiles_input = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/volatiles_input");

    public static Texture neutrality_base = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/neutrality/base");
    public static Texture cyclicity_base = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/cyclicity/base");

    public static Texture[] oxygen_iris = PrimaCyclia.GetAnimation("textures/prima_cyclia/parts/iris_oxygen_full.array", "iris",16);
    public static Texture[] phlog_iris = PrimaCyclia.GetAnimation("textures/prima_cyclia/parts/iris_phlogiston_full.array", "iris",16);

    public static List<AtomType> primeAtoms = [
        AtomTypes.quicksilver,
        AtomTypes.mors,
        Atoms.Sulfur,
        AtomTypes.vitae
    ];
    public static List<AtomType> volatileAtoms = [
        Atoms.Sulfur,
        Atoms.Oxygen,
        Atoms.Phlogiston
    ];

    static Molecule TolomeoMolecule() {
        Molecule molecule = new();
        molecule.AddAtom(new Atom(AtomTypes.vitae), new HexIndex(0, 1));
        molecule.AddAtom(new Atom(AtomTypes.salt), new HexIndex(1, 0));
        molecule.AddAtom(new Atom(AtomTypes.quicksilver), new HexIndex(1, -1));
        molecule.AddAtom(new Atom(AtomTypes.mors), new HexIndex(0, -1));
        molecule.AddAtom(new Atom(AtomTypes.salt), new HexIndex(-1, 0));
        molecule.AddAtom(new Atom(Atoms.Sulfur), new HexIndex(-1, 1));
        return molecule;
    }

    public static Sound CombustionOxygenRelease, CombustionPhlogRelease, CombustionBothRelease, CyclicitySound, EmpowermentConsume, EmpowermentProduce, NeutralitySound, PolaritySound, PrimalitySound, SublimationSound;
    public static Sound[] EmpowermentBonds, SublimationProduction;
    public static void LoadSounds() {
        CombustionOxygenRelease = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/combustion_oxyg");
        CombustionPhlogRelease = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/combustion_phlog");
        CombustionBothRelease = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/combustion_both");
        CyclicitySound = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/cyclicity_sound");
        EmpowermentConsume = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/empowerment_oxyg_in");
        EmpowermentProduce = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/empowerment_phlog_out");
        NeutralitySound = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/neutrality_sound");
        PolaritySound = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/polarity_sound");
        PrimalitySound = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/primality_sound");
        SublimationSound = AssetLoaderHelper.LoadSound("sounds/prima_cyclia/sublimation_sound");
        EmpowermentBonds = [
            AssetLoaderHelper.LoadSound("sounds/prima_cyclia/empowerment_supertriplex_a"),
            AssetLoaderHelper.LoadSound("sounds/prima_cyclia/empowerment_supertriplex_b"),
            AssetLoaderHelper.LoadSound("sounds/prima_cyclia/empowerment_supertriplex_c")
        ];
        SublimationProduction = [
            AssetLoaderHelper.LoadSound("sounds/prima_cyclia/sublimation_out_1_or_2"),
            AssetLoaderHelper.LoadSound("sounds/prima_cyclia/sublimation_out_3_or_4"),
            AssetLoaderHelper.LoadSound("sounds/prima_cyclia/sublimation_out_5_or_6")
        ];

        Volume.SetVolume(CombustionOxygenRelease, 0.3f);
        Volume.SetVolume(CombustionPhlogRelease, 0.3f);
        Volume.SetVolume(CombustionBothRelease, 0.3f);
        Volume.SetVolume(CyclicitySound, 0.3f);
        Volume.SetVolume(EmpowermentConsume, 0.3f);
        Volume.SetVolume(EmpowermentProduce, 0.3f);
        Volume.SetVolume(NeutralitySound, 0.3f);
        Volume.SetVolume(PolaritySound, 0.3f);
        Volume.SetVolume(PrimalitySound, 0.3f);
        Volume.SetVolume(SublimationSound, 0.3f);
        Volume.SetVolume(EmpowermentBonds[0], 0.3f);
        Volume.SetVolume(EmpowermentBonds[1], 0.3f);
        Volume.SetVolume(EmpowermentBonds[2], 0.3f);
        Volume.SetVolume(SublimationProduction[0], 0.3f);
        Volume.SetVolume(SublimationProduction[1], 0.3f);
        Volume.SetVolume(SublimationProduction[2], 0.3f);
    }

    public static void drawSelectionGlow(SolutionEditorBase seb_self, Part part, Vector2 pos, float alpha) {
        var cageSelectGlowTexture = Assets.textures.select.cage;
        int armLength = 1; // part.method_1165()
        IntermediatePartState class236 = seb_self.GetIntermState(part, pos);
        Color color = Color.White.WithAlpha(alpha);

        patch_SolutionEditorBase.RenderArmSelection(armLength, HexArmRotations, class236, color);
        for (int index = 0; index < 6; ++index) {
            float num = index * sixtyDegrees;
            patch_SolutionEditorBase.RenderRotated(cageSelectGlowTexture, color, class236.pos, class236.rotation + num);
        }
    }

    private static AtomType GetPrime(int i) {
        while (i < 0) {i += 4;}
        while (i > 3) {i -= 4;}
        return primeAtoms[i];
    }

    private static Molecule GetTolomeoWheelAtoms(PartSimState state) => GetTolomeoWheelData(state, TolomeoWheelAtomsField, TolomeoMolecule());
    public static Maybe<AtomReference> MaybeFindTolomeoWheelAtom(Sim sim_self, Part part, HexIndex offset) => MaybeFindTolomeoWheelAtom(sim_self, part.GlobalPosFromLocal(offset));
    public static Maybe<AtomReference> MaybeFindTolomeoWheelAtom(Sim sim_self, HexIndex hex) {
        var SEB = sim_self.solutionEditor;
        var solution = SEB.GetSolution();
        var partList = solution.parts;
        var partSimStates = sim_self.simulationDict;

        foreach (var Tolomeo in partList.Where(x => x.GetType() == Tolomeo)) {
            var partSimState = partSimStates[Tolomeo];
            Molecule TolomeoAtoms = GetTolomeoWheelAtoms(partSimState);
            var hexIndex = partSimState.newPosition;
            var rotation = partSimState.rotation;
            var hexKey = (hex - hexIndex).Rotated(rotation.Negative());

            if (TolomeoAtoms.GetAtoms().TryGetValue(hexKey, out Atom atom)) {
                return new AtomReference(TolomeoAtoms, hexKey, atom.atomType, atom, true);
            }
        }
        return MaybeHelper.empty;
    }

    private static void SetTolomeoWheelData<T>(PartSimState state, string field, T data) => new DynamicData(state).Set(field, data);
    private static T GetTolomeoWheelData<T>(PartSimState state, string field, T initial) {
        var data = new DynamicData(state).Get(field);
        if (data == null) {
            SetTolomeoWheelData(state, field, initial);
            return initial;
        }
        return (T)data;
    }

    static bool ContentLoaded = false;
    public static void AddPartTypes() {        

        if (ContentLoaded) throw new Exception( "Prima Cyclia tried to load content multiple times, this shouldn't happen." );
        ContentLoaded = true;
        //=========================//

        Tolomeo = new PartType() {
            cost = 30,
            layer = PartTypeLayer.Top,
            isProgrammable = true,
            canRotateInSimulation = true,
            attachedAtoms = [],
            iconTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/tolomeo"),
            iconHoverTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/tolomeo_hover"),
            isUnique = true,
            CustomPermissionCheck = perms => perms.Contains(PrimaCyclia.Instance.GetIdentifier("tolomeos_wheel"))
        };
        Tolomeo.attachedAtoms.Add(new HexIndex(0, 1), AtomTypes.vitae);
        Tolomeo.attachedAtoms.Add(new HexIndex(1, 0), AtomTypes.salt);
        Tolomeo.attachedAtoms.Add(new HexIndex(1, -1), AtomTypes.quicksilver);
        Tolomeo.attachedAtoms.Add(new HexIndex(0, -1), AtomTypes.mors);
        Tolomeo.attachedAtoms.Add(new HexIndex(-1, 0), AtomTypes.salt);
        Tolomeo.attachedAtoms.Add(new HexIndex(-1, 1), Atoms.Sulfur);

        QApi.AddPartTypeToPanel(Tolomeo, PartTypes.berlosWheel);
        PrimaCyclia.Instance.AddPuzzlePermission("tolomeos_wheel");
        PrimaCyclia.Instance.AddPartType(Tolomeo, "tolomeos_wheel", RenderTolomeoPart);

        Primality = new() {
            cost = 20,
            isFullHexCover = true,
            glowTexture = Assets.textures.select.double_glow,
            strokeTexture = Assets.textures.select.double_stroke,
            //iconTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/primality"),
            //iconHoverTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/primality_hover"),
            glyphHexes = [center, hex_right],
            permissionCategory = PuzzlePermissions.None,
            CustomPermissionCheck = perms => perms.Contains(PrimaCyclia.Instance.GetIdentifier("primality"))
        };
        QApi.AddPartTypeToPanel(Primality, false);
        PrimaCyclia.Instance.AddPuzzlePermission("primality");
        PrimaCyclia.Instance.AddPartType(Primality, "primality", RenderPrimality);
        QApi.BindGlyphCycle(Primality, new(PartCycleDelegate.PartCycleExecutionType.Normal, PrimalityOnCycle));
        PrimaCyclia.Instance.AddRecipe(new(), Primality.Id, Primality.Id);

        Neutrality = new() {
            cost = 15,
            isFullHexCover = true,
            glowTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/neutrality/glow"),
            strokeTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/neutrality/stroke"),
            //iconTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/neutrality"),
            //iconHoverTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/neutrality_hover"),
            glyphHexes = [center, hex_right, hex_left],
            permissionCategory = PuzzlePermissions.None,
            CustomPermissionCheck = perms => perms.Contains(PrimaCyclia.Instance.GetIdentifier("neutrality"))
        };
        QApi.AddPartTypeToPanel(Neutrality, false);
        PrimaCyclia.Instance.AddPuzzlePermission("neutrality");
        PrimaCyclia.Instance.AddPartType(Neutrality, "neutrality", RenderNeutrality);
        QApi.BindGlyphCycle(Neutrality, new(PartCycleDelegate.PartCycleExecutionType.Normal, NeutralityOnCycle));
        PrimaCyclia.Instance.AddRecipe(new(), Neutrality.Id, Neutrality.Id);

        Cyclicity = new() {
            cost = 20,
            isFullHexCover = true,
            glowTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/cyclicity/glow"),
            strokeTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/cyclicity/stroke"),
            //iconTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/cyclicity"),
            //iconHoverTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/cyclicity_hover"),
            glyphHexes = [center, hex_up_right, hex_down_right],
            permissionCategory = PuzzlePermissions.None,
            CustomPermissionCheck = perms => perms.Contains(PrimaCyclia.Instance.GetIdentifier("cyclicity"))
        };
        QApi.AddPartTypeToPanel(Cyclicity, false);
        PrimaCyclia.Instance.AddPuzzlePermission("cyclicity");
        PrimaCyclia.Instance.AddPartType(Cyclicity, "cyclicity", RenderCyclicity);
        QApi.BindGlyphCycle(Cyclicity, new(PartCycleDelegate.PartCycleExecutionType.Normal, CyclicityOnCycle));
        PrimaCyclia.Instance.AddRecipe(new(), Cyclicity.Id, Cyclicity.Id);

        Polarity = new() {
            cost = 15,
            isFullHexCover = true,
            glowTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/neutrality/glow"),
            strokeTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/neutrality/stroke"),
            //iconTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/polarity"),
            //iconHoverTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/polarity_hover"),
            glyphHexes = [center, hex_left, hex_right],
            permissionCategory = PuzzlePermissions.None,
            CustomPermissionCheck = perms => perms.Contains(PrimaCyclia.Instance.GetIdentifier("polarity"))
        };
        QApi.AddPartTypeToPanel(Polarity, false);
        PrimaCyclia.Instance.AddPuzzlePermission("polarity");
        PrimaCyclia.Instance.AddPartType(Polarity, "polarity", RenderPolarity);
        QApi.BindGlyphCycle(Polarity, new(PartCycleDelegate.PartCycleExecutionType.Normal, PolarityOnCycle));
        PrimaCyclia.Instance.AddRecipe(new(), Polarity.Id, Polarity.Id);

        Combustion = new() {
            cost = 20,
            isFullHexCover = true,
            glowTexture = AssetLoaderHelper.LoadTexture("textures/select/diamond_glow"),
            strokeTexture = AssetLoaderHelper.LoadTexture("textures/select/diamond_stroke"),
            //iconTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/combustion"),
            //iconHoverTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/combustion_hover"),
            glyphHexes = [center, hex_up_right, hex_down_right, hex_right],
            permissionCategory = PuzzlePermissions.None,
            CustomPermissionCheck = perms => perms.Contains(PrimaCyclia.Instance.GetIdentifier("combustion"))
        };
        QApi.AddPartTypeToPanel(Combustion, false);
        PrimaCyclia.Instance.AddPuzzlePermission("combustion");
        PrimaCyclia.Instance.AddPartType(Combustion, "combustion", CombustionGlyph.Render);
        QApi.BindGlyphCycle(Combustion, new(PartCycleDelegate.PartCycleExecutionType.Normal, CombustionGlyph.OnCycle));
        PrimaCyclia.Instance.AddRecipe(new(), Combustion.Id, Combustion.Id);

        Sublimation = new() {
            cost = 20,
            isFullHexCover = true,
            glowTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/sublimation/glow"),
            strokeTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/sublimation/stroke"),
            //iconTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/sublimation"),
            //iconHoverTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/sublimation_hover"),
            glyphHexes = [ center, hex_left, hex_right, hex_down_left, hex_down_right, hex_up_left, hex_up_right,
                SublimationGlyph.sublimationIrises[0], SublimationGlyph.sublimationIrises[1], SublimationGlyph.sublimationIrises[2],
                SublimationGlyph.sublimationIrises[3], SublimationGlyph.sublimationIrises[4], SublimationGlyph.sublimationIrises[5]
            ],
            permissionCategory = PuzzlePermissions.None,
            isUnique = true,
            CustomPermissionCheck = perms => perms.Contains(PrimaCyclia.Instance.GetIdentifier("sublimation"))
        };
        QApi.AddPartTypeToPanel(Sublimation, false);
        PrimaCyclia.Instance.AddPuzzlePermission("sublimation");
        PrimaCyclia.Instance.AddPartType(Sublimation, "sublimation", SublimationGlyph.Render);
        QApi.BindGlyphCycle(Sublimation, new(PartCycleDelegate.PartCycleExecutionType.Normal, SublimationGlyph.OnCycle));
        PrimaCyclia.Instance.AddRecipe(new(), Sublimation.Id, Sublimation.Id);

        Empowerment = new() {
            cost = 10,
            isFullHexCover = true,
            glowTexture = Assets.textures.select.disposal_glow,
            strokeTexture = Assets.textures.select.disposal_stroke,
            //iconTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/empowerment"),
            //iconHoverTexture = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/parts/icons/empowerment_hover"),
            glyphHexes = [hex_left, hex_right, hex_up_left, hex_up_right],
            permissionCategory = PuzzlePermissions.None,
            CustomPermissionCheck = perms => perms.Contains(PrimaCyclia.Instance.GetIdentifier("empowerment"))
        };
        QApi.AddPartTypeToPanel(Empowerment, false);
        PrimaCyclia.Instance.AddPuzzlePermission("empowerment");
        PrimaCyclia.Instance.AddPartType(Empowerment, "empowerment", EmpowermentGlyph.Render);
        QApi.BindGlyphCycle(Empowerment, new(PartCycleDelegate.PartCycleExecutionType.Normal, EmpowermentGlyph.OnCycle));
        PrimaCyclia.Instance.AddRecipe(new(), Empowerment.Id, Empowerment.Id);
    }

    static bool PrimalityOnCycle(Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) {
        if (sim.GetAtomReference(part, hex_right, false, out AtomReference converted) && converted.atomType == AtomTypes.salt) {
            if (sim.GetAtomReference(part, center, true, out AtomReference check) && primeAtoms.Contains(check.atomType)) {
                converted.molecule.ReplaceAtom(check.atomType, converted.pos);
                converted.atom.transmutationEffect = new TransmutationEffect(sim.solutionEditor, TransmutationEffectRenderMode.AsMask, converted.atomType, Assets.textures.atoms.duplicate_mask, 30f);
                Vector2 param_5374 = HexGrid.standardGrid.ToPixelCoords(part.GetHexPos() + new HexIndex(1, 0).Rotated(part.GetRotation()));
                sim.solutionEditor.underneathPartsGlyphEffects.Add(new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime, param_5374, Assets.textures.parts.projection_glyph_flash, 30f, Vector2.Zero, part.GetRotation().ToRadians()));
                PrimalitySound.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                return true;
            }
        }
        return false;
    }
    static bool NeutralityOnCycle(Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) {
        if (isCycleStart && !simState.isProcessing) {
            if (sim.GetAtomReference(part, hex_left, false, out AtomReference atom_1) && !atom_1.inMultiAtomMolecule && !atom_1.isHeldByArm &&
                sim.GetAtomReference(part, hex_right, false, out AtomReference atom_2) && !atom_2.inMultiAtomMolecule && !atom_2.isHeldByArm &&
                ((atom_1.atomType == Atoms.Sulfur && atom_2.atomType == AtomTypes.quicksilver) ||
                 (atom_2.atomType == Atoms.Sulfur && atom_1.atomType == AtomTypes.quicksilver))
            ) {
                if (!sim.HasAtomAt(part, center, true)) {
                    atom_1.molecule.RemoveAtom(atom_1.pos);
                    sim.solutionEditor.consumptionEffects.Add(new ConsumptionEffect(sim.solutionEditor, atom_1));
                    atom_2.molecule.RemoveAtom(atom_2.pos);
                    sim.solutionEditor.consumptionEffects.Add(new ConsumptionEffect(sim.solutionEditor, atom_2));
                    simState.isProcessing = true;
                    sim.additionalCollisions.Add(new(Sim.ColliderType.Atom, HexGrid.standardGrid.ToPixelCoords(part.GetHexPos())) { radius = 15f });
                    NeutralitySound.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                    return true;
                }
            }
        } else if (simState.isProcessing) {
            Molecule m = new();
            m.AddAtom(new Atom(AtomTypes.salt), part.GetHexPos());
            sim.molecules.Add(m);
            return true;
        }
        return false;
    }
    static bool CyclicityOnCycle(Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) {
        if (isCycleStart && !simState.isProcessing) {
            if (sim.GetAtomReference(part, hex_up_right, false, out AtomReference atom_1) && !atom_1.inMultiAtomMolecule && !atom_1.isHeldByArm && primeAtoms.Contains(atom_1.atomType)) {
                if (!sim.HasAtomAt(part, hex_down_right, true)) {
                    atom_1.molecule.RemoveAtom(atom_1.pos);
                    sim.solutionEditor.consumptionEffects.Add(new ConsumptionEffect(sim.solutionEditor, atom_1));
                    simState.isProcessing = true;
                    sim.additionalCollisions.Add(new(Sim.ColliderType.Atom, HexGrid.standardGrid.ToPixelCoords(part.GlobalPosFromLocal(hex_down_right))) { radius = 15f });
                    CyclicitySound.PlaySound(sim.solutionEditor.GetVolumeMultiplier());

                    simState.processingAtoms = [GetPrime(primeAtoms.IndexOf(atom_1.atomType) - 1)];
                    return true;
                }
            }
        } else if (simState.isProcessing) {
            Molecule m = new();
            m.AddAtom(new Atom(simState.processingAtoms[0]), part.GlobalPosFromLocal(hex_down_right));
            sim.molecules.Add(m);
            return true;
        }
        return false;
    }
    static bool PolarityOnCycle(Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart) {
        if (sim.GetAtomReference(part, hex_left, false, out AtomReference input) && input.atomType == AtomTypes.salt && !input.inMultiAtomMolecule && !input.isHeldByArm) {
            if (sim.GetAtomReference(part, hex_right, true, out AtomReference transmuted) && primeAtoms.Contains(transmuted.atomType)) {
                input.molecule.RemoveAtom(input.pos);
                sim.solutionEditor.consumptionEffects.Add(new ConsumptionEffect(sim.solutionEditor, input));
                transmuted.molecule.ReplaceAtom(GetPrime(primeAtoms.IndexOf(transmuted.atomType) + 2), transmuted.pos);
                transmuted.atom.transmutationEffect = new TransmutationEffect(sim.solutionEditor, TransmutationEffectRenderMode.AsMask, transmuted.atomType, Assets.textures.atoms.duplicate_mask, 30f);
                Vector2 param_5374 = HexGrid.standardGrid.ToPixelCoords(part.GetHexPos() + new HexIndex(1, 0).Rotated(part.GetRotation()));
                sim.solutionEditor.underneathPartsGlyphEffects.Add(new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime, param_5374, Assets.textures.parts.projection_glyph_flash, 30f, Vector2.Zero, part.GetRotation().ToRadians()));
                PolaritySound.PlaySound(sim.solutionEditor.GetVolumeMultiplier());
                return true;
            }
        }
        return false;
    }

    static void RenderTolomeoPart(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {
        // draw atoms, if the simulation is stopped - otherwise, the running simulation will draw them
        if (editor.GetSimPlayState() == SimPlayState.Stopped) {
            drawTolomeoAtoms(editor, part, pos);
        }

        // draw arm stubs
        IntermediatePartState class236 = editor.GetIntermState(part, pos);
        patch_SolutionEditorBase.RenderArmLights(part.GetArmLength(), HexArmRotations, class236);

        // draw cages
        PartSimState partSimState = editor.GetSimulation().GetSimState(part);
        for (int i = 0; i < 6; i++) {
            float radians = renderer.partRotation + (i * sixtyDegrees);
            Vector2 vector2_9 = renderer.partPos + hexGraphicalOffset(new HexIndex(1, 0)).Rotated(radians);
            patch_SolutionEditorBase.RenderLights(atomCageLighting, vector2_9, new Vector2(39f, 33f), radians);
        }
    }
    static void RenderPrimality(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {
        Vector2 vector10 = new(41f, 48f);
        renderer.RenderBase(Assets.textures.parts.projection_glyph.@base, new Vector2(-1f, -1f), vector10, 0f);
        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, center, 3f);
        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, hex_right, 3f);
        renderer.RenderRotating(Assets.textures.parts.calcinator_bowl, center, Vector2.Zero);
        renderer.RenderUpright(prime_symbols, center, new Vector2(0f, 0f));
        renderer.RenderRotating(Assets.textures.parts.projection_glyph.metal_bowl, hex_right, Vector2.Zero);
        renderer.RenderBase(Assets.textures.parts.duplicator_bond, vector10 + new Vector2(-72f, -27f));
    }
    static void RenderNeutrality(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {
        Vector2 offset = new(123f, 48f);

        renderer.RenderBase(neutrality_base, Vector2.Zero, offset, 0f);

        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, center, 3f);
        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, hex_left, 3f);
        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, hex_right, 3f);

        renderer.RenderRotating(Assets.textures.parts.projection_glyph.quicksilver_input, hex_left, Vector2.Zero); //input bowl 1
        renderer.RenderRotating(Assets.textures.parts.projection_glyph.quicksilver_input, hex_right, Vector2.Zero); //input bowl 2
        renderer.RenderUpright(neutrality_input, hex_left, Vector2.Zero);
        renderer.RenderUpright(neutrality_input, hex_right, Vector2.Zero);

        PartSimState pss = editor.GetSimulation().GetSimState(part);
        IntermediatePartState uco = editor.GetIntermState(part, pos);
        float time = editor.GetCycleTime();

        int irisFrame = 15;
        bool afterIrisOpens = false;
        Molecule risingAtom = Molecule.CreateMonoatomic(AtomTypes.salt);

        Vector2 risingOffset = uco.pos + HexGrid.standardGrid.ToPixelCoords(center).Rotated(uco.rotation);

        if (pss.isProcessing) {
            irisFrame = Utils.Clamp((int)(Utils.InterpolateLinear(1f, -1f, time) * 16f), 0, 15);
            afterIrisOpens = time > 0.5f;
            if (!afterIrisOpens) {
                // show atom rising behind iris
                Editor.RenderMolecule(risingAtom, risingOffset, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
            }
        }
        renderer.RenderUpright(Assets.textures.parts.iris_full[irisFrame], center, Vector2.Zero);
        renderer.RenderRotating(Assets.textures.parts.animismus.output_above_iris, center, Vector2.Zero);
        if (pss.isProcessing && afterIrisOpens) {
            // show atom rising infront of iris
            Editor.RenderMolecule(risingAtom, risingOffset, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
        }
    }
    static void RenderCyclicity(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {
        Vector2 offset = new(82f, 120f);

        renderer.RenderBase(cyclicity_base, Vector2.Zero, offset, 0f);

        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, hex_up_right, 3f);
        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, hex_down_right, 3f);

        renderer.RenderRotating(Assets.textures.parts.projection_glyph.quicksilver_input, hex_up_right, Vector2.Zero); //input bowl 1
        renderer.RenderUpright(primes_input, hex_up_right, Vector2.Zero);

        PartSimState pss = editor.GetSimulation().GetSimState(part);
        IntermediatePartState uco = editor.GetIntermState(part, pos);
        float time = editor.GetCycleTime();

        int irisFrame = 15;
        bool afterIrisOpens = false;
        Molecule risingAtom = pss.isProcessing ? Molecule.CreateMonoatomic(pss.processingAtoms[0]) : Molecule.CreateMonoatomic(primeAtoms[0]);

        Vector2 risingOffset = uco.pos + HexGrid.standardGrid.ToPixelCoords(hex_down_right).Rotated(uco.rotation);

        if (pss.isProcessing) {
            irisFrame = Utils.Clamp((int)(Utils.InterpolateLinear(1f, -1f, time) * 16f), 0, 15);
            afterIrisOpens = time > 0.5f;
            if (!afterIrisOpens) {
                // show atom rising behind iris
                Editor.RenderMolecule(risingAtom, risingOffset, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
            }
        }
        renderer.RenderUpright(Assets.textures.parts.iris_full[irisFrame], hex_down_right, Vector2.Zero);
        renderer.RenderRotating(Assets.textures.parts.animismus.output_above_iris, hex_down_right, Vector2.Zero);
        if (pss.isProcessing && afterIrisOpens) {
            // show atom rising infront of iris
            Editor.RenderMolecule(risingAtom, risingOffset, new HexIndex(0, 0), 0f, 1f, time, 1f, false, null);
        }
    }
    static void RenderPolarity(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer) {
        Vector2 offset = new(123f, 48f);

        renderer.RenderBase(neutrality_base, Vector2.Zero, offset, 0f);
        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, hex_left, 3f);
        renderer.RenderShadowCircle(Assets.textures.parts.bonder_shadow, hex_right, 3f);
        renderer.RenderRotating(Assets.textures.parts.calcinator_bowl, hex_right, Vector2.Zero);
        renderer.RenderRotating(Assets.textures.parts.projection_glyph.quicksilver_input, hex_left, Vector2.Zero); //input bowl 1
        renderer.RenderUpright(Assets.textures.parts.animismus.symbol_salt, hex_left, new Vector2(2, 0));
        renderer.RenderUpright(prime_symbols, hex_right, Vector2.Zero);
        //renderer.RenderBase(Assets.textures.parts.field_173, offset + new Vector2(-72f, -27f));
    }
    public static void drawTolomeoAtoms(SolutionEditorBase seb_self, Part part, Vector2 pos) {
        if (part.GetType() != Tolomeo) return;
        PartSimState partSimState = seb_self.GetSimulation().GetSimState(part);

        IntermediatePartState class236 = seb_self.GetIntermState(part, pos);
        Editor.RenderMolecule(GetTolomeoWheelAtoms(partSimState), class236.pos, center, class236.rotation, 1f, 1f, 1f, false, seb_self);
    }
}