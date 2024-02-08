using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

/*
 * [0.0.1]
 * First release.
*/

namespace ShowWeight {
	[BepInPlugin("fiote.mods.showweight", "ShowWeight", "0.0.1")]

	public class ShowWeight : BaseUnityPlugin {
		// core stuff
		public static bool debug = true;
		public static ConfigFile configFile;

		public static GameObject goWeight;
		public static TextMeshProUGUI txtWeight;

		public static ConfigEntry<int> configTextOffsetX;
		public static ConfigEntry<int> configTextOffsetY;
		public static ConfigEntry<int> configTextFontSize;

		public static ConfigEntry<HorizontalAnchor> configHorizontalAnchor;
		public static ConfigEntry<VerticalAnchor> configVerticalAnchor;

		private void Awake() {
			Bar();
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "fiote.mods.showweight");
			Debug("Awake");
			SetupConfig();
			Bar();
		}

		void SetupConfig() {
			Line();
			Debug("SetupConfig()");
			
			configFile = Config;
			Config.Bind("General", "NexusID", 2696, "NexusMods ID for updates.");

			configHorizontalAnchor = Config.Bind("Anchoring", "HorizontalAnchor", HorizontalAnchor.LEFT, "Adjust the weight's text horizontal anchor.");
			configVerticalAnchor = Config.Bind("Anchoring", "VerticalAnchor", VerticalAnchor.BOTTOM, "Adjust the weight's text vertical anchor.");
			configTextOffsetX = Config.Bind("Text", "OffsetX", 40, "Adjust the weight's text horizontal position (left/right).");
			configTextOffsetY = Config.Bind("Text", "OffsetY", 20, "Adjust the weight's text vertical position (down/up).");
			configTextFontSize = Config.Bind("Text", "FontSize", 20, "Adjust the weight's text font size.");
			configFile.SettingChanged += (sender, e) => Recreate();
		}

		public static void Recreate() {
			Line();
			Debug("Recreate()");
			CreateWeightText(null);
		}	

		public static void CreateWeightText(Hud hud) {
			Line();
			Debug("CreateWeightText()");
			if (hud == null) hud = Hud.instance;

			var parent = hud?.m_rootObject;
			if (parent == null) return;

			if (goWeight != null) {
				GameObject.Destroy(goWeight);
			}
			
			goWeight = new GameObject("ShowWeightText", typeof(RectTransform));
			goWeight.transform.SetParent(parent.transform, false);


			// bottom-left corner... i think
			var goRect = goWeight.transform as RectTransform;
			goRect.sizeDelta = new Vector2(200f, 30f);
			goRect.localPosition = new Vector2(configTextOffsetX.Value, configTextOffsetY.Value);
			Vector2 pivot = new Vector2(0.5f, 0.5f);

			switch (configHorizontalAnchor.Value) {
				case HorizontalAnchor.LEFT:
					pivot.x = 0f;
					break;
				case HorizontalAnchor.CENTER:
					pivot.x = 0.5f;
					break;
				case HorizontalAnchor.RIGHT:
					pivot.x = 1f;
					break;
			}

			switch (configVerticalAnchor.Value) {
				case VerticalAnchor.TOP:
					pivot.y = 1f;
					break;
				case VerticalAnchor.CENTER:
					pivot.y = 0.5f;
					break;
				case VerticalAnchor.BOTTOM:
					pivot.y = 0f;
					break;
			}


			goRect.anchorMin = pivot;
			goRect.anchorMax = pivot;
			goRect.pivot = pivot;

			txtWeight = goWeight.AddComponent<TextMeshProUGUI>();
			txtWeight.fontSize = configTextFontSize.Value;
			txtWeight.font = hud?.m_healthText?.font;
			txtWeight.text = "...";
			txtWeight.alignment = ConvertAnchorToAlignment(configHorizontalAnchor.Value, configVerticalAnchor.Value);

			UpdateTexts();
			Line();
		}

		public static void InventoryChanged(Inventory inventory) {
			if (Player.m_localPlayer == null) return;
			if (inventory == Player.m_localPlayer.GetInventory()) UpdateTexts();
		}

		public static void UpdateTexts() {
			if (Player.m_localPlayer == null) return;
			Player player = Player.m_localPlayer;
			int carrying = Mathf.CeilToInt(player.GetInventory().GetTotalWeight());
			int maxweight = Mathf.CeilToInt(player.GetMaxCarryWeight());
			float pr = (float)carrying / (float)maxweight;
			txtWeight.text = "Weight: "+carrying+"/"+maxweight;
		}
		
		static TextAlignmentOptions ConvertAnchorToAlignment(HorizontalAnchor horizontalAnchor, VerticalAnchor verticalAnchor) {
			switch (horizontalAnchor) {
				case HorizontalAnchor.LEFT:
					switch (verticalAnchor) {
						case VerticalAnchor.TOP:
							return TextAlignmentOptions.TopLeft;
						case VerticalAnchor.CENTER:
							return TextAlignmentOptions.MidlineLeft;
						case VerticalAnchor.BOTTOM:
							return TextAlignmentOptions.BottomLeft;
					}
					break;
				case HorizontalAnchor.CENTER:
					switch (verticalAnchor) {
						case VerticalAnchor.TOP:
							return TextAlignmentOptions.Top;
						case VerticalAnchor.CENTER:
							return TextAlignmentOptions.Midline;
						case VerticalAnchor.BOTTOM:
							return TextAlignmentOptions.Bottom;
					}
					break;
				case HorizontalAnchor.RIGHT:
					switch (verticalAnchor) {
						case VerticalAnchor.TOP:
							return TextAlignmentOptions.TopRight;
						case VerticalAnchor.CENTER:
							return TextAlignmentOptions.MidlineRight;
						case VerticalAnchor.BOTTOM:
							return TextAlignmentOptions.BottomRight;
					}
					break;
			}

			return TextAlignmentOptions.TopLeft; // Default value
		}

		#region LOGGING

		public static void Bar() {
			Debug("=============================================================");
		}

		public static void Line() {
			Debug("-------------------------------------------------------------");
		}

		public static void Debug(string message) {
			if (debug) Log(message);
		}

		public static void Log(string message) {
			UnityEngine.Debug.Log($"[ShowWeight] {message}");
		}

		public static void Error(string message) {
			UnityEngine.Debug.LogError($"[ShowWeight] {message}");
		}

		#endregion
	}
}

public class ConfigJson {
	public bool pinless;
}

public enum HorizontalAnchor {
	LEFT,
	CENTER,
	RIGHT
}

public enum VerticalAnchor {
	TOP,
	CENTER,
	BOTTOM
}