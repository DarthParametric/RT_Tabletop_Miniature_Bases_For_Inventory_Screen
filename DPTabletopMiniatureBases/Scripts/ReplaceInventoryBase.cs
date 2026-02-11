using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Localization;
using Kingmaker.Modding;
using Kingmaker.Settings;
using Kingmaker.UI.Models.SettingsUI;
using Kingmaker.UI.DollRoom;
using Newtonsoft.Json;
using Owlcat.Runtime.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DPTabletopMiniatureBases
{
    [HarmonyPatch]
    public static class ReplaceInventoryBase
    {
        internal static LogChannel Logger;
        internal static OwlcatModification DPMod;
        internal static string DPBaseName;
        internal static string DPBaseAssetID;
		internal static int BaseDiameter;

        public static bool IsEnabled;
        public static Settings settings;
        
		public static string[] BaseTypes =
		{
			"Glossy Black",
			"Pseudo Flock",
			"Diamondplate",
			"Metal Tiles",
			"Ceramic Tiles",
			"Stylised Wood",
			"Stylised Stone"
		};

        internal static Dictionary<string, string> Base_IDs_10mm_Dict = new()
        {
            {"40K_Base_10mm_Plain_Black", "ae2607d5dd153da41836a7ce320c108f"},
            {"40K_Base_10mm_Flocked", "310af23644f85dc4690c4ee9ab4090e2"},
            {"40K_Base_10mm_Diamondplate", "dd88be47170d4634f98a5341a3cc53cf"},
            {"40K_Base_10mm_Metal_Plates", "6a597f10375ec1f40b38ea0b82dd9637"},
			{"40K_Base_10mm_Tiles_Terracotta", "7a70cde171e96b544ad183961db7e831"},
			{"40K_Base_10mm_Wood_Stylised", "f1a892ae5cc86c9418a7e22290a0db8a"},
			{"40K_Base_10mm_Stone", "7239881edcb232047a8f9b316c65b60b"},
        };
		
        internal static Dictionary<string, string> Base_IDs_15mm_Dict = new()
        {
            {"40K_Base_15mm_Plain_Black", "e063c2efe849fa5409af7357b3a11a10"},
            {"40K_Base_15mm_Flocked", "19bc94527a32917488bd898e60e2bd28"},
            {"40K_Base_15mm_Diamondplate", "6bfb345fbd9541046a199d7bf7537ca9"},
            {"40K_Base_15mm_Metal_Plates", "9ebd46ad8bfc4ae499fd73cf35f514f6"},
			{"40K_Base_15mm_Tiles_Terracotta", "94ed7d461c4a4bc4ab83f2fbac1861ff"},
			{"40K_Base_15mm_Wood_Stylised", "f450222d72d9fc746b34510c2af2e4bb"},
			{"40K_Base_15mm_Stone", "1da9897d335be4b45b108064be853bea"},
        };

        [OwlcatModificationEnterPoint]
        public static void EnterPoint(OwlcatModification modification)
        {
            try
            {
                DPMod = modification;
                Logger = DPMod.Logger;
                settings = DPMod.LoadData<Settings>();
				LogDebug("Loaded mod settings");
                DPMod.OnGUI += OnGUI;
                IsEnabled = true;

                Harmony harmony = new(DPMod.Manifest.UniqueName);
                harmony.PatchAll();
            }
            catch (Exception ex)
            {
                Logger.Log($"Caught an exception in EnterPoint: \n{ex}");
            }
        }

        public static void OnGUI()
        {
            try
            {
                GUILayout.Label("<b>Enable Character Base Swapping:</b>", GUILayout.ExpandWidth(false));
                GUILayout.BeginHorizontal();
                settings.ReplaceActive = GUILayout.Toggle(settings.ReplaceActive, "Replace Bases");
                GUILayout.EndHorizontal();

                GUILayout.Space(15);
				
				var stylecentered = new GUIStyle(GUI.skin.toggle) { alignment = TextAnchor.MiddleCenter };
				stylecentered.onNormal.textColor = Color.green;

				GUILayout.Label("<b>Choose Base Type:</b>", GUILayout.ExpandWidth(false));
				GUILayout.BeginHorizontal();
				settings.SelectedBaseType = GUILayout.SelectionGrid(settings.SelectedBaseType, BaseTypes, 8, stylecentered);
                GUILayout.EndHorizontal();

                GUILayout.Space(15);

                GUILayout.Label("<b>Disable Dollroom Post-Process Effects:</b>", GUILayout.ExpandWidth(false));
                GUILayout.Label("(Removes scanlines, distortion, servoskull, and background image)", GUILayout.ExpandWidth(false));
                GUILayout.BeginHorizontal();
                settings.PostProcessDisabled = GUILayout.Toggle(settings.PostProcessDisabled, "Disable Post-Process");
                GUILayout.EndHorizontal();

                GUILayout.Space(15);
				
				/*
				// HIDE THIS OPTION IN THE GUI, REQUIRE MANUAL EDITING OF CONFIG FILE TO ENABLE.
                GUILayout.Label("<b>Enable Detailed Logging:</b>", GUILayout.ExpandWidth(false));
                GUILayout.Label("(Logspam for debugging purposes)", GUILayout.ExpandWidth(false));
                GUILayout.BeginHorizontal();
                settings.DetailedLogging = GUILayout.Toggle(settings.DetailedLogging, "Detailed Logs");
                GUILayout.EndHorizontal();

                GUILayout.Space(15);
				*/

                if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false)))
                {
                    DPMod.SaveData(settings);
					LogDebug("Saved mod settings");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Caught an exception in OnGUI: \n{ex}");
            }
        }

        public static void LogDebug(string message)
        {
            if (settings.DetailedLogging)
            {
                Logger.Log($"DEBUG: {message}");
            }
        }

        public static string GetBaseNameFromIndex(int index, int basesize)
        {
            var Base10 = Base_IDs_10mm_Dict.Keys.ElementAt(index);
			var Base15 = Base_IDs_15mm_Dict.Keys.ElementAt(index);

			if (basesize == 15)
			{
				return Base15;
			}
			else
			{
				return Base10;
			}
        }
		
        public static string GetBaseAssetIDFromIndex(int index, int basesize)
        {
            var Base10 = Base_IDs_10mm_Dict.Values.ElementAt(index);
			var Base15 = Base_IDs_15mm_Dict.Values.ElementAt(index);

			if (basesize == 15)
			{
				return Base15;
			}
			else
			{
				return Base10;
			}
        }
        
		[HarmonyPatch(typeof(CharacterDollRoom), "SetupUnit")]
		[HarmonyPostfix]
		static void Dollroom_Postfix(CharacterDollRoom __instance)
        {
			LogDebug("Dollroom_Postfix patch started");
            
            try
            {
                //LogDebug("Checking for CharacterPlaceholder");
                Transform TargetPlaceholder = (AccessTools.Field(typeof(CharacterDollRoom), "m_TargetPlaceholder").GetValue(__instance) as Transform);
				Transform DollRoomRoot = TargetPlaceholder.transform.parent.transform;
				Transform DollRoomLightRoot = DollRoomRoot.Find("DollRoomLightSetup");
				Transform UpskirtLight = DollRoomLightRoot.Find("DollRoomLight_SpotUnderCharacterHolo");
				Transform InvPostProcess = DollRoomLightRoot.Find("DollRoom_PostProcessSettings");
				Transform CharGenPostProcess = DollRoomLightRoot.Find("DollRoomCharGen_PostProcessSettings");
				
                if (TargetPlaceholder != null)
                {
					LogDebug("------------------------------------------------------------------------");
					
					BaseDiameter = 10;
					
					foreach (Transform child in TargetPlaceholder)
					{
						var name = child.name;

						if (name == "ServoskullLookTarget" || name == "DollRoomCharacterStand" || name.Contains("40K_Base_")) continue;
						
						if (name.Contains("Spacemarine") || name.Contains("UrolonDarkApostle") || name.Contains("CyberMastiff"))
						{
							BaseDiameter = 15;
							LogDebug($"Found large base target {name}, setting base diameter to {BaseDiameter}.");
						}
						else
						{
							BaseDiameter = 10;
							LogDebug($"Found regular base target {name}, setting base diameter to {BaseDiameter}.");
						}
					}
					
					LogDebug("------------------------------------------------------------------------");
					
					LogDebug($"Base diameter is set to {BaseDiameter}mm");
					
					//LogDebug($"Found valid {TargetPlaceholder.name}, checking for DollRoomCharacterStand");
					
					Transform CharacterStand = TargetPlaceholder.Find("DollRoomCharacterStand");

                    if (CharacterStand != null)
                    {
                        if (settings.ReplaceActive)
                        {
                            DPBaseName = GetBaseNameFromIndex(settings.SelectedBaseType, BaseDiameter);
                            LogDebug($"Base swapping setting is active, using base {DPBaseName}");
							
							string BaseClone = DPBaseName + "(Clone)";
							
							if (CharacterStand.gameObject.activeSelf)
							{
								LogDebug($"Found active {CharacterStand.name}, deactivating");
								CharacterStand.gameObject.SetActive(false);
							}
							
							// ACCOUNT FOR A PRE-EXISTING BASE, CHARACTER SIZE, OR THE USER CHANGING THE BASE TYPE FROM THE OPTIONS MENU MID-SESSION.
							foreach (Transform child in TargetPlaceholder)
							{
								//LogDebug($"Found child of {TargetPlaceholder.name} - {child.name}");
								
								if (child.name.Contains("40K_Base_") && child.name != BaseClone)
								{
									LogDebug($"Found existing base of different type ({child.name.Remove(child.name.Length - 7)}) from setting ({DPBaseName}), destroying");
									GameObject.Destroy(child.gameObject);
								}
							}
							
							Transform NewBase = TargetPlaceholder.Find(BaseClone);
							
							if (NewBase == null)
							{
								DPBaseAssetID = GetBaseAssetIDFromIndex(settings.SelectedBaseType, BaseDiameter);
								LogDebug($"Loading {DPBaseName} ({DPBaseAssetID}) from bundle");
								
								var BasePrefab = ResourcesLibrary.TryGetResource<GameObject>(DPBaseAssetID, false, true);
								
								LogDebug($"Instantiating {BasePrefab.name} as child of {TargetPlaceholder.name}");
								UnityEngine.Object.Instantiate(BasePrefab, TargetPlaceholder);
								
								//LogDebug($"Adjusting local position of {BasePrefab.name} {BasePrefab.transform.localPosition} to {CharacterStand.transform.localPosition}");
								BasePrefab.transform.localPosition = CharacterStand.transform.localPosition;
								
								if (UpskirtLight != null && UpskirtLight.gameObject.activeSelf)
								{
									LogDebug($"Found active {UpskirtLight.name}, deactivating");
									UpskirtLight.gameObject.SetActive(false);
								}
							}
							else
							{
								LogDebug($"{NewBase.name.Remove(NewBase.name.Length - 7)} already instantiated, skipping");
							}
                        }
                        else
                        {
                            LogDebug($"Swapping bases set to disabled, reverting to vanilla character stand");
							
							foreach (Transform child in TargetPlaceholder)
							{
								if (child.name.Contains("40K_Base_"))
								{
									GameObject.Destroy(child.gameObject);
								}
							}
							
							CharacterStand.gameObject.SetActive(true);
							UpskirtLight.gameObject.SetActive(true);
                        }
						
						if (InvPostProcess != null && CharGenPostProcess != null)
						{
							//LogDebug($"Found valid {InvPostProcess.name}");
							//LogDebug($"Found valid {CharGenPostProcess.name}");
							
							if (settings.PostProcessDisabled)
							{
								if (InvPostProcess.gameObject.activeSelf || CharGenPostProcess.gameObject.activeSelf)
								{
									// DEACTIVATES POST-PROCESS OBJECT - DISABLES SCANLINE VFX, DISTORTION, AND SERVOSKULL.
									InvPostProcess.gameObject.SetActive(false);
									CharGenPostProcess.gameObject.SetActive(false);
									LogDebug($"Disable Post-Process setting is {settings.PostProcessDisabled}, disabling active post-process objects.");
								}
							}
							else
							{
								if (!InvPostProcess.gameObject.activeSelf || !CharGenPostProcess.gameObject.activeSelf)
								{
									// SET POST-PROCESS TO ACTIVE IN CASE THEY WERE PREVIOUSLY DEACTIVATED.
									LogDebug($"Disable Post-Process setting is {settings.PostProcessDisabled}, enabling inactive post-process objects.");
									InvPostProcess.gameObject.SetActive(true);
									CharGenPostProcess.gameObject.SetActive(true);
								}
							}
						}
						
						LogDebug("===========================================================================");
                    }
                    else
                    {
                        LogDebug("DollRoomCharacterStand not found!");
                    }
                }
                else
                {
                    LogDebug("CharacterPlaceholder not found!");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Caught an exception in DollRoom_Postfix:\n{ex}");
            }
        }
    }

    [Serializable]
    public class Settings
    {
        public bool ReplaceActive = true;
		public bool DetailedLogging = false;
        public bool PostProcessDisabled = false;
        public int SelectedBaseType = 1;
    }
}