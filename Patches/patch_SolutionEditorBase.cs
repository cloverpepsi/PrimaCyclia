using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using PrimaCyclia;
using System.Linq;

internal abstract class patch_SolutionEditorBase : SolutionEditorBase {
    [MonoModIgnore]
    public extern void orig_RenderGlow(Part part, Vector2 pos, float alpha);

    public void RenderGlow(Part part, Vector2 pos, float alpha) {
        if (part.GetType() == Parts.Tolomeo) Parts.drawSelectionGlow(this, part, pos, alpha);
        orig_RenderGlow(part, pos, alpha);
    }

    [MonoModIgnore]
    [MonoModInternalM]
    internal static extern void RenderArmSelection(int length, HexRotation[] rotations, IntermediatePartState intermState, Color color);
    [MonoModIgnore]
    [MonoModInternalM]
    internal static extern void RenderRotated(Texture texture, Color color, Vector2 pos, float angle);
    [MonoModIgnore]
    [MonoModInternalM]
    internal static extern void RenderArmLights(int length, HexRotation[] rotations, IntermediatePartState intermState);
    [MonoModIgnore]
    [MonoModInternalM]
    internal static extern void RenderLights(LightDirTextures lightTextures, Vector2 pos, Vector2 centerOffset, float rotation);

    [MonoModILInject("RenderSolution")]
    static void PatchDrawWheelAtoms(MethodDefinition method, CustomAttribute attribute) {
        ILCursor cursor = new(new ILContext(method));
        MethodDefinition toCall = method.DeclaringType.Methods.First(m => m.Name.Equals("prima_cycliaRenderTolomeoWheel"));

        // skip ahead to roughly where 'RenderMoleculeWithHolder' is called
        cursor.Goto(600);
        // jump ahead to just after the 'RenderMoleculeWithHolder' for-loop
        cursor.GotoNext(MoveType.Before, instr => instr.MatchCallvirt("SolutionEditorBase", "RenderMoleculeWithHolder"));
        cursor.GotoPrev(MoveType.Before, instr => instr.MatchCallvirt("ISim", "GetMolecules"));
        cursor.GotoPrev(MoveType.After, instr => instr.MatchLdarg0());

        // load the SolutionEditorBase self and the class423 local onto the stack so we can use it
        cursor.Emit(OpCodes.Ldloc_0);
        // then run the new code
        cursor.EmitCall(toCall);
        // Add back the Ldarg_0 borrowed from above
        cursor.Emit(OpCodes.Ldarg_0);
    }

    private void prima_cycliaRenderTolomeoWheel(SolutionEditorBase.class_423 class423) {
        if (GetSimPlayState() != SimPlayState.Stopped) {
            var partList = GetSolution().parts;
            foreach (var servin in partList.Where(x => x.GetType() == Parts.Tolomeo)) {
                Parts.drawTolomeoAtoms(this, servin, class423.boardOffset);
            }
        }
    }
}
