using Quintessential;

namespace PrimaCyclia;

internal static class Atoms {
	
	public static AtomType Sulfur, Oxygen, Phlogiston;

	public static void AddAtomTypes() {
        Sulfur = new() {
            symbol = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/atoms/sulfur_symbol"),
            shadow = Assets.textures.atoms.shadow,
            matteTextures = new MatteAtomTextures {
                diffuse = Assets.textures.atoms.salt_diffuse,
                shade = Assets.textures.atoms.salt_shade
            },
        };
        PrimaCyclia.Instance.AddAtomType(Sulfur, "sulfur");

        Oxygen = new() {
            symbol = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/atoms/oxygen_symbol"),
            shadow = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/atoms/oxygen_shadow"),
            matteTextures = new MatteAtomTextures {
                diffuse = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/atoms/oxygen_diffuse"),
                shade = Assets.textures.atoms.elements.specular_highlight
            },
        };
        PrimaCyclia.Instance.AddAtomType(Oxygen, "oxygen");

        Phlogiston = new() {
            symbol = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/atoms/phlogiston_symbol"),
            shadow = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/atoms/phlogiston_shadow"),
            matteTextures = new MatteAtomTextures {
                diffuse = AssetLoaderHelper.LoadTexture("textures/prima_cyclia/atoms/phlogiston_diffuse"),
                shade = Assets.textures.atoms.elements.specular_highlight
            },
        };
        PrimaCyclia.Instance.AddAtomType(Phlogiston, "phlogiston");

        QApi.AddMoleculeName(Sulfur, PrimaCyclia.Instance.Translate("molecule_prefix.sulfur"));

        QApi.AddMoleculeName(Oxygen, PrimaCyclia.Instance.Translate("molecule_prefix.oxygen"));

        QApi.AddMoleculeName(Phlogiston, PrimaCyclia.Instance.Translate("molecule_prefix.phlogiston"));
    }
}