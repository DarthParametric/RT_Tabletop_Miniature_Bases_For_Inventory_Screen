# Tabletop Miniature Bases
A mod for Owlcat's Rogue Trader cRPG. Replaces the flat holographic disc in the inventory and character creation screens with a classic tabletop-style miniature base.

<p align="center"><img src="img/Header_alt.jpg?raw=true"/></p>

## Overview
This mod adds a selection of tabletop-style miniature bases that replace the holographic disc that characters stand on in the inventory and character creation screens. Several different variants are provided, selectable via the mod manager. You can also optionally disable the green scanline VFX and floating servoskull in these screens.

## Installation
This is an Owlmod, made using the Unity template supplied by Owlcat. In order to properly load custom assets, you ***must*** install the Unity Mod Manager-based mod [MicroPatches](https://github.com/microsoftenator2022/MicroPatches/releases) by microsoftenator2022. You can install it manually or via [ModFinder RT](https://www.nexusmods.com/warhammer40kroguetrader/mods/146).

Use [ModFinder RT](https://www.nexusmods.com/warhammer40kroguetrader/mods/146) to install this mod automagically.

Alternatively, to install the mod manually, first make sure you have run the game at least once. Download the mod from the [releases section](https://github.com/DarthParametric/RT_Tabletop_Miniature_Bases_For_Inventory_Screen/releases/latest) or [Nexus](https://www.nexusmods.com/warhammer40kroguetrader/mods/406) and extract it into:

`%userprofile%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\Modifications\`

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

## User Settings
You can configure the mod's settings via the Owlcat Mod Manager (Shift-F10 by default). Click on the Settings button next to the mod's name in the list, which will open its settings page. Here you can enable or disable base swapping, choose the type of base you want to use, and whether you want to disable the post-process effects such as the green scanlines, the distortion effect, and the floating servoskull in the background. Once you have changed the settings to your desired values, hit the Apply button at the bottom to save them. 

Your settings will be saved in a `DPTabletopMiniatureBases_Data.json` file in the Modifications folder. The settings apply universally across all saves.

## Known Issues
Choosing the option to disable post-process effects will also disable the background image, leaving your character in a black void. This is unavoidable as they are all linked together in a single post-process object.

The reflection probe in the scene does not capture dynamic objects (and trying to enable the option didn't work), so the character won't show up in any reflections on the glossy surfaces of the base. This is particularly apparent on the plain black base. The top surface looks kind of weird because of that.

Larger characters, such as Ulfar and Glaito, automatically switch to using a larger diameter base. On rare occasions, equipping a weapon may cause the base to switch to the smaller one. I _think_ I have prevented this from re-occurring, but it may just be difficult to reproduce.

## Acknowledgements
Many thanks to the modders on the Owlcat Discord, but particularly microsoftenator2022, Kurufinve, and ADDB for helping to coach me through my ineptitude in order to get the mod working.
