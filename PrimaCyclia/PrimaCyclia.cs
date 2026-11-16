using Quintessential;
using System.IO;

namespace PrimaCyclia;

public class PrimaCyclia : QuintessentialMod {
    public static PrimaCyclia Instance { get; }
    public static Quintessential.Logging.Logger Logger => Instance.Log;
    public override string ModId => "prima_cyclia";
    
    public override void Load() { }

    public override void LoadContent() {
        Atoms.AddAtomTypes();
        Parts.LoadSounds();
        Parts.AddPartTypes();
    }
    public override void LoadCompatContent() { }
    public override void FinaliseContent() { }
    public override void PostLoad() { }
    public override void Unload() { }

    public static Texture[] GetAnimation(string containingFolder, string frameBaseName, int frameCount, int padding = 4) {
        Texture[] anim = new Texture[frameCount];
        for (int i = 0; i < frameCount; i++) {
            anim[i] = AssetLoaderHelper.LoadTexture(Path.Combine(containingFolder, frameBaseName) + "_" + (i + 1).ToString().PadLeft(padding, '0'));
        }
        return anim;
    }
}
