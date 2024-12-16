using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Modding;
using Kingmaker.UI.DollRoom;
using Owlcat.Runtime.Core.Logging;
using System;
using System.Collections.Generic;
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

		internal static Dictionary<string, string> Base_IDs_Dict = new()
	    {
			{"DP_40K_Base_Aquila", "9e4d3b7f727f0a74d9dac32e44ae7433"},
			{"DP_40K_Base_Diamondplate", "92fcf2fba6fdc8046832d2594668eefb"},
		    {"DP_40K_Base_Flocked", "6d5beadf7f9522c48af97df396e52adf"},
		    {"DP_40K_Base_Metal_Plate", "2ae8f87a8aed9ae43bab6bde3203561b"},
		    {"DP_40K_Base_Plain_Black", "e17d069f0721bea40b864d68fb5e7070"},
		    {"DP_40K_Base_Tiles_Cracked", "9c1cbcee6b84ffb4196f4864c24075b3"},
		    {"DP_40K_Base_Tiles_Terracotta", "56deacec01d0be74fb65ce05b1352298"},
	    };

		[OwlcatModificationEnterPoint]
		public static void EnterPoint(OwlcatModification modification)
		{
			DPMod = modification;
			Logger = DPMod.Logger;

			Harmony harmony = new(DPMod.Manifest.UniqueName);
			harmony.PatchAll();
		}

		public static void LogDebug(string message)
		{
#if DEBUG
			Logger.Log($"DEBUG: {message}");
#endif
		}

		[HarmonyPatch(typeof(CharacterDollRoom), nameof(CharacterDollRoom.Show))]
		[HarmonyPostfix]
		static void DollRoom_Postfix(CharacterDollRoom __instance)
		{
			Logger.Log("CharacterDollRoom loaded, running patch");
			try
			{
				LogDebug("Checking for CharacterPlaceholder");
				Transform TargetPlaceholder = (AccessTools.Field(typeof(CharacterDollRoom), "m_TargetPlaceholder").GetValue(__instance) as Transform);

				if (TargetPlaceholder != null)
				{
					LogDebug($"Found valid {TargetPlaceholder.name}, checking for DollRoomCharacterStand");
					Transform CharacterStand = TargetPlaceholder.Find("DollRoomCharacterStand");

					if (CharacterStand != null)
					{
						if (Settings.Initialized)
						{
							DPBaseName = Settings.GetBaseType();
							LogDebug($"Applying CharacterStand from ModMenu setting, value is {DPBaseName}");
						}
						else 
						{
							DPBaseName = "DP_40K_Base_Plain_Black";
						}
						
						string BaseClone = DPBaseName + "(Clone)";
						
						LogDebug($"Found valid {CharacterStand.name}, deactivating");
						CharacterStand.gameObject.SetActive(false);

						// ACCOUNT FOR A PRE-EXISTING BASE AND/OR THE USER CHANGING THE BASE TYPE FROM THE OPTIONS MENU MID-SESSION.
						foreach (Transform child in TargetPlaceholder)
						{
							LogDebug($"Found child of {TargetPlaceholder.name} - {child.name}");

							if (child.name.Contains("DP_40K_Base_") && child.name != BaseClone)
							{
								LogDebug($"Found existing base of different type ({child.name.Remove(child.name.Length-7)}) from settings ({DPBaseName}), destroying");
								GameObject.Destroy(child.gameObject);
							}
						}

						Transform NewBase = TargetPlaceholder.Find(BaseClone);

						if (NewBase == null)
						{
							DPBaseAssetID = Base_IDs_Dict[DPBaseName];
							LogDebug($"Loading {DPBaseName} ({DPBaseAssetID}) from bundle");

							var BasePrefab = ResourcesLibrary.TryGetResource<GameObject>(DPBaseAssetID, false, true);

							LogDebug($"Instantiating {BasePrefab.name} as child of {TargetPlaceholder.name}");
							UnityEngine.Object.Instantiate(BasePrefab, TargetPlaceholder);
							
							LogDebug($"Adjusting local position of {BasePrefab.name} {BasePrefab.transform.localPosition} to {CharacterStand.transform.localPosition}");
							BasePrefab.transform.localPosition = CharacterStand.transform.localPosition;

							try
							{
								Transform DollRoomRoot = TargetPlaceholder.transform.parent.transform;
								Transform DollRoomLightRoot = DollRoomRoot.Find("DollRoomLightSetup");
								Transform UpskirtLight = DollRoomLightRoot.Find("DollRoomLight_SpotUnderCharacterHolo");

								if (UpskirtLight != null)
								{
									LogDebug($"Found valid {UpskirtLight.name}, deactivating");
									UpskirtLight.gameObject.SetActive(false);
								}

								Transform InvPostProcess = DollRoomLightRoot.Find("DollRoom_PostProcessSettings");
								Transform CharGenPostProcess = DollRoomLightRoot.Find("DollRoomCharGen_PostProcessSettings");

								if (InvPostProcess != null && CharGenPostProcess != null)
								{
									LogDebug($"Found valid {InvPostProcess.name}");
									LogDebug($"Found valid {CharGenPostProcess.name}");

									if (Settings.Initialized && Settings.IsPostProcessEnabled())
									{
										// DEACTIVATES POST-PROCESS OBJECT - DISABLES SCANLINE VFX, DISTORTION, AND SERVOSKULL.
										InvPostProcess.gameObject.SetActive(false);
										CharGenPostProcess.gameObject.SetActive(false);
									}
								}
							}
							catch (Exception e)
							{
								Logger.Log($"Encountered an exception attempting to disable DollRoomLight_SpotUnderCharacterHolo / DollRoom_PostProcessSettings:\n{e}");
							}
						}
						else
						{
							LogDebug($"{NewBase.name} already instantiated, skipping");
						}
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
			catch (Exception e)
			{
				Logger.Log($"Encountered an exception:\n{e}");
			}
		}
	}
}