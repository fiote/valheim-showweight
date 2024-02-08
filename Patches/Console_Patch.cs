using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShowWeight {

	[HarmonyPatch(typeof(Console), "InputText")]
	public static class Console_InputText_Patch {

		public class ConsoleValue {
			public string value;
			public float floatvalue;
			public int intvalue;
			public bool boolvalue;

			public ConsoleValue(string value) {
				this.value = value.ToLower();
				var floated = float.TryParse(value, out floatvalue);
				intvalue = floated ? Mathf.RoundToInt(floatvalue) : 0;
				boolvalue = (value == "1" || value == "yes" || value == "on");
			}
		}

		public static Dictionary<string, ConsoleValue> vars = new Dictionary<string, ConsoleValue>() {
			{  "x" , new ConsoleValue("20") },
			{  "y" , new ConsoleValue("-10") },
			{  "f" , new ConsoleValue("20") },
		};

		static bool Prefix(Console __instance) {
			string text = __instance.m_input.text;
			var parts = text.Split('=');

			if (parts.Length == 2) {
				var key = parts[0];

				var value = parts[1];
				var cvalue = new ConsoleValue(value);
				var keyparts = key.Split('.');

				if (keyparts[0] != "sw") {
					vars[key] = cvalue;
					ShowWeight.Recreate();
					return true;
				}

				var cmd = keyparts[1];

				// ===== ANCHORING ================================================

				if (cmd == "h") {
					Enum.TryParse(cvalue.value.ToUpper(), out HorizontalAnchor anchor);
					ShowWeight.configHorizontalAnchor.Value = anchor;
					ShowWeight.Log("HORIZONTAL ANCHOR config changed to " + anchor);
				}

				if (cmd == "v") {
					Enum.TryParse(cvalue.value.ToUpper(), out VerticalAnchor anchor);
					ShowWeight.configVerticalAnchor.Value = anchor;
					ShowWeight.Log("VERTICAL ANCHOR config changed to " + anchor);
				}

				// ===== WEIGHT TEXT ==============================================

				if (cmd == "fontsize") {
					ShowWeight.configTextFontSize.Value = cvalue.intvalue;
					ShowWeight.Log("FONTSIZE config changed to " + cvalue.intvalue);
				}
				if (cmd == "x") {
					ShowWeight.configTextOffsetX.Value = cvalue.intvalue;
					ShowWeight.Log("OFFSETX config changed to " + cvalue.intvalue);
				}
				if (cmd == "y") {
					ShowWeight.configTextOffsetY.Value = cvalue.intvalue;
					ShowWeight.Log("OFFSETY config changed to " + cvalue.intvalue);
				}

				ShowWeight.configFile.Save();
				ShowWeight.Recreate();
				return false;
			}

			return true;
		}
	}
}