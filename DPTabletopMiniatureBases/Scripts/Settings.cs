using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Localization;
using Kingmaker.UI.Models.SettingsUI.SettingAssets.Dropdowns;
using ModMenu.Settings;
using MMSettings = ModMenu.Settings;
using Owlcat.Runtime.Core.Logging;
using UnityEngine;

namespace DPTabletopMiniatureBases
{
    public static class Settings
    {
        internal static LogChannel Logger = ReplaceInventoryBase.Logger;
        public static bool Initialized = false;
        private static readonly string RootKey = "dptabletopminaturebases";
        public static readonly string DropdownKey = "dropdown";
        public static readonly string DropdownKey2 = "dropdown2";
        public static readonly string PostProcessKey = "toggleakey";
        public static readonly string ToggleBKey = "togglebkey";
        private static readonly LocalizedString DPModName = new() { Key = "DPTabletopMiniatureBases_ModName" };
        private static readonly LocalizedString DPModDesc = new() { Key = "DPTabletopMiniatureBases_ModDesc" };
        private static readonly LocalizedString HeaderA = new() { Key = "DPTabletopMiniatureBases_HeaderA" };
        private static readonly LocalizedString HeaderB = new() { Key = "DPTabletopMiniatureBases_HeaderB" };
        private static readonly LocalizedString DropdownDesc = new() { Key = "DPTabletopMiniatureBases_DropdownDesc" };
        private static readonly LocalizedString DropdownDescLong = new() { Key = "DPTabletopMiniatureBases_DropdownDescLong" };
        private static readonly LocalizedString DropdownList1 = new() { Key = "DPTabletopMiniatureBases_DropdownList1" };
        private static readonly LocalizedString DropdownList2 = new() { Key = "DPTabletopMiniatureBases_DropdownList2" };
        private static readonly LocalizedString DropdownList3 = new() { Key = "DPTabletopMiniatureBases_DropdownList3" };
        private static readonly LocalizedString DropdownList4 = new() { Key = "DPTabletopMiniatureBases_DropdownList4" };
        private static readonly LocalizedString DropdownList5 = new() { Key = "DPTabletopMiniatureBases_DropdownList5" };
        private static readonly LocalizedString DropdownList6 = new() { Key = "DPTabletopMiniatureBases_DropdownList6" };
        private static readonly LocalizedString DropdownList7 = new() { Key = "DPTabletopMiniatureBases_DropdownList7" };
        private static readonly LocalizedString PostProcessDesc = new() { Key = "DPTabletopMiniatureBases_ToggleADesc" };
        private static readonly LocalizedString BasesOffDesc = new() { Key = "DPTabletopMiniatureBases_ToggleBDesc" };
        private static readonly LocalizedString PostProcessDescLong = new() { Key = "DPTabletopMiniatureBases_ToggleADescLong" };
        private static readonly LocalizedString BasesOffDescLong = new() { Key = "DPTabletopMiniatureBases_ToggleBDescLong" };

        private enum BaseTypeEnum
        {
            DP_40K_Base_Aquila,
            DP_40K_Base_Diamondplate,
            DP_40K_Base_Flocked,
            DP_40K_Base_Metal_Plate,
            DP_40K_Base_Plain_Black,
            DP_40K_Base_Tiles_Cracked,
            DP_40K_Base_Tiles_Terracotta
        }

        private class UISettingsEntityDropdownBaseTypeEnum : UISettingsEntityDropdownEnum<BaseTypeEnum> { }

        public static void Init()
        {
            if (Initialized)
            {
                ReplaceInventoryBase.LogDebug("ModMenu settings already initialised");
                return;
            }

            Logger.Log("Initializing ModMenu Settings");

            ModMenu.ModMenu.AddSettings(
                SettingsBuilder.New(RootKey, DPModName)
                    .SetMod(ReplaceInventoryBase.DPMod)
                    .SetModDescription(DPModDesc)
                    .AddSubHeader(HeaderA, true)
                    .AddDropdown(
                        Dropdown<BaseTypeEnum>.New(
                            GetKey(DropdownKey),
                            BaseTypeEnum.DP_40K_Base_Plain_Black,
                            DropdownDesc,
                            ScriptableObject.CreateInstance<UISettingsEntityDropdownBaseTypeEnum>())
                        .WithLongDescription(DropdownDescLong)
                        .DependsOnSave()
                    )
                    .AddDropdownList(
                        MMSettings.DropdownList.New(
                            GetKey(DropdownKey2),
                            defaultSelected: 0,
                            DropdownDesc,
                            new()
                                {
                                    DropdownList5, // DP_40K_Base_Plain_Black
								    DropdownList3, // DP_40K_Base_Flocked
								    DropdownList1, // DP_40K_Base_Aquila
								    DropdownList2, // DP_40K_Base_Diamondplate
								    DropdownList4, // DP_40K_Base_Metal_Plate
								    DropdownList6, // DP_40K_Base_Tiles_Cracked
								    DropdownList7, // DP_40K_Base_Tiles_Terracotta
                                }
                            )
                        .WithLongDescription(DropdownDescLong)
                        .DependsOnSave()
                    )
                    .AddSubHeader(HeaderB, true)
                    .AddToggle(
                        MMSettings.Toggle.New(GetKey(PostProcessKey), defaultValue: false, PostProcessDesc)
                        .WithLongDescription(PostProcessDescLong)
                    )
                    .AddToggle(
                        MMSettings.Toggle.New(GetKey(ToggleBKey), defaultValue: false, BasesOffDesc)
                        .WithLongDescription(BasesOffDescLong)
                    )
            );

            Initialized = true;
        }

        internal static bool IsPostProcessEnabled()
        {
            return ModMenu.ModMenu.GetSettingValue<bool>(GetKey(PostProcessKey));
        }

        internal static bool IsCombatLogDebugEnabled()
        {
            return ModMenu.ModMenu.GetSettingValue<bool>(GetKey("consoledebug"));
        }

        private static string GetKey(string partialKey)
        {
            return $"{RootKey}.{partialKey}";
        }

        public static string GetBaseType()
        {
            var SelectedBase = ModMenu.ModMenu.GetSettingValue<BaseTypeEnum>(GetKey(DropdownKey)).ToString();
            ReplaceInventoryBase.LogDebug($"ModMenu selected dropdown value is {SelectedBase}");

            return SelectedBase;
        }
    }

    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(BlueprintsCache))]
    static class BlueprintsCache_Postfix
    {
        [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
        static void Postfix()
        {
			ReplaceInventoryBase.LogDebug("Running ModMenu settings Harmony patch.");
			Settings.Init();
        }
    }
}