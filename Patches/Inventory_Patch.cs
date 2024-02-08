using HarmonyLib;

namespace ShowWeight.Patches {

	[HarmonyPatch(typeof(Inventory), "UpdateTotalWeight")]
	public static class Inventory_UpdateTotalWeight_Patch {
		public static void Postfix(Inventory __instance) {
			ShowWeight.InventoryChanged(__instance);
		}
	}
}