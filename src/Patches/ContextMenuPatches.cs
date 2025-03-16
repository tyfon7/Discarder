using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using TMPro;
using UIFixesInterop;

namespace Discarder;

public static class ContextMenuPatches
{
    public static void Enable()
    {
        new ContextMenuNamesPatch().Enable();
    }

    public class ContextMenuNamesPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ContextMenuButton), nameof(ContextMenuButton.Show));
        }

        [PatchPostfix]
        public static void Postfix(string caption, TextMeshProUGUI ____text)
        {
            if (MultiSelect.Count > 0 && caption == EItemInfoButton.Discard.ToString())
            {
                ____text.text += " (x" + MultiSelect.Count + ")";
            }
        }
    }
}
