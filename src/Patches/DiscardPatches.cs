using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using System.Threading.Tasks;
using UIFixesInterop;

namespace Discarder;

public static class DiscardPatches
{
    public static void Enable()
    {
        new DiscardPatch().Enable();
    }

    public class DiscardPatch : ModulePatch
    {
        private static bool InPatch = false;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ItemUiContext), nameof(ItemUiContext.ThrowItem));
        }

        [PatchPrefix]
        public static bool Prefix(ItemUiContext __instance, ref Task __result)
        {
            if (InPatch || MultiSelect.Count <= 1)
            {
                return true;
            }

            InPatch = true;

            __result = MultiSelect.Apply(__instance.ThrowItem, __instance)
                .ContinueWith(t => { InPatch = false; });

            return false;
        }
    }
}