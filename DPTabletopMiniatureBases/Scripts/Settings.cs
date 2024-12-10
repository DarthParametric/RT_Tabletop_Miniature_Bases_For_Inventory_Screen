using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Localization;
using Kingmaker.Modding;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UI.Models.SettingsUI.SettingAssets.Dropdowns;
using ModMenu.NewTypes;
using ModMenu.Settings;
using Owlcat.Runtime.Core.Logging;
using System.Text;
using KeyBinding = ModMenu.Settings.KeyBinding;
using UnityEngine;

namespace DPTabletopMiniatureBases
{
    public static class Settings
    {
        internal static LogChannel Logger = ReplaceInventoryBase.Logger;
        private static bool Initialized = false;
        private static readonly string RootKey = "dptabletopminaturebases";
        public static readonly string DropdownKey = "dropdown";
        private static readonly LocalizedString DPModName = new() { Key = "DPTabletopMiniatureBases_ModName" };
        private static readonly LocalizedString DPModDesc = new() { Key = "DPTabletopMiniatureBases_ModDesc" };
        private static readonly LocalizedString HeaderA = new() { Key = "DPTabletopMiniatureBases_HeaderA" };
        private static readonly LocalizedString DropdownDesc = new() { Key = "DPTabletopMiniatureBases_DropdownDesc" };
        private static readonly LocalizedString DropdownDescLong = new() { Key = "DPTabletopMiniatureBases_DropdownDescLong" };
        //private static readonly SettingsBuilder settings = SettingsBuilder.New(RootKey, DPModName);
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
#if DEBUG
                Logger.Log("ModMenu settings already initialised");
#endif
                return;
            }

            Logger.Log("Initializing ModMenu Settings");

            ModMenu.ModMenu.AddSettings(
                SettingsBuilder.New(RootKey, DPModName)
                    .SetMod(ReplaceInventoryBase.DPMod)
                    .SetModDescription(DPModDesc)
                    .AddSubHeader(HeaderA)
                    .AddDropdown(
                        Dropdown<BaseTypeEnum>.New(
                            GetKey(DropdownKey),
                            BaseTypeEnum.DP_40K_Base_Plain_Black,
                            DropdownDesc,
                            ScriptableObject.CreateInstance<UISettingsEntityDropdownBaseTypeEnum>())
                        .ShowVisualConnection()
                        .WithLongDescription(DropdownDescLong)
                        .DependsOnSave()
                    )
            );

            Initialized = true;
        }

        private static string GetKey(string partialKey)
        {
            return $"{RootKey}.{partialKey}";
        }

        public static string GetBaseType()
        {
            var SelectedBase = ModMenu.ModMenu.GetSettingValue<BaseTypeEnum>(GetKey(DropdownKey)).ToString();
#if DEBUG
            Logger.Log($"ModMenu selected dropdown value is {SelectedBase}");
#endif
            return SelectedBase;
        }
    }

    [HarmonyPatch(typeof(BlueprintsCache))]
    static class BlueprintsCache_Postfix
    {
        [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
        static void Postfix()
        {
#if DEBUG
            ReplaceInventoryBase.Logger.Log("Running ModMenu settings Harmony patch.");
#endif
            Settings.Init();
        }
    }
}