using HarmonyLib;

namespace ShowWeight.Patches {

	[HarmonyPatch(typeof(Hud), "Awake")]
	public static class Hud_Awake_Patch {
		public static void Postfix(Hud __instance) {
			ShowWeight.CreateWeightText(__instance);
		}
	}
}