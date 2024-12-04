# Tabletop Miniature Bases For Inventory Screen
A mod for Owlcat's Rogue Trader cRPG. Replaces the flat holographic disc in the inventory and character creation screens with a classic tabletop-style miniature base.

## Overview
This mod adds a selection of tabletop-style miniature bases that replace the holographic disc that characters stand on in the inventory and character creation screens. Several different variants are provided, selectable via the mod manager. Additionally, you can also optionally disable the green scanline VFX and floating servoskull in these screens.

## Installation
This is an Owlmod, made using the Unity template supplied by Owlcat. Currently the game has a bug that prevents Owlmods from working. To fix this, you ***must*** install the Unity Mod Manager-based mod [MicroPatches](https://github.com/microsoftenator2022/MicroPatches/releases) by microsoftenator2022. You can install it manually or via [ModFinder](https://www.nexusmods.com/warhammer40kroguetrader/mods/146).

To install this mod, first make sure you have run the game at least once. Download the archive and extract it into:

%LocalAppData%Low\Owlcat Games\Warhammer 40000 Rogue Trader\Modifications\
Each Owlmod needs to be in its own sub-folder in the Modifications folder.

Afterwards, you need to edit OwlcatModificationManagerSettings.json in the base Warhammer 40000 Rogue Trader folder in a text editor (Notepad++ recommended). It should look something like this:

```
{
"$id": "1",
"SourceDirectories": [],
"EnabledModifications": ["DPTabletopMiniatureBases"],
"ActiveModifications": ["DPTabletopMiniatureBases"],
"DisabledModifications": []
}
```

If you have other mods, list them in quotes separated by commas. For example:

```
"EnabledModifications": ["DPTabletopMiniatureBases", "ModName2", "ModName3"],
"ActiveModifications": ["DPTabletopMiniatureBases", "ModName2", "ModName3"],
```

You can move individual mods from the ActiveModifications section to the DisabledModifications section if you want to disable them without physically removing them.

Alternatively, use [ModFinder](https://www.nexusmods.com/warhammer40kroguetrader/mods/146) to install the mod.

It should be fine to install the mod in an existing game at any point right up to completing Argenta's quest in chapter 4 (or after if you're willing to use Toybox to manually spawn the helmets).

## Known Issues
Choosing the option to disable the scanline VFX and servoskull will also disable the background image, leaving your character in a black void. This is unavoidable as they are all linked together in a single post-processing object.

## Acknowledgements
Many thanks to the modders on the Owlcat Dicord, but particularly microsoftenator2022 and Kurufinve, for helping to coach me through my ineptitude in order to get the mod working.
