using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UIFixesInterop;

namespace Discarder;

public static class DiscardPatches
{
    private static bool InMultiDiscard = false;
    private static bool DialogShown = false;

    public static void Enable()
    {
        new DiscardPatch().Enable();
        new DiscardDialogPatch().Enable();
    }

    // Calls ItemUiContext.ThrowItem() for each item in the multi-selection
    public class DiscardPatch : ModulePatch
    {

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ItemUiContext), nameof(ItemUiContext.ThrowItem));
        }

        [PatchPrefix]
        public static bool Prefix(ItemUiContext __instance, Item item, ref Task __result)
        {
            if (InMultiDiscard || MultiSelect.Count <= 1 || !MultiSelect.Items.Contains(item))
            {
                return true;
            }

            InMultiDiscard = true;

            __result = MultiSelect.Apply(__instance.ThrowItem, __instance)
                .ContinueWith(t =>
                {
                    InMultiDiscard = false;
                    DialogShown = false;
                });

            return false;
        }
    }

    // Only show the confirm dialog once
    public class DiscardDialogPatch : ModulePatch
    {
        private static bool Result = false;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.FirstMethod(typeof(ItemUiContext), m => m.Name == nameof(ItemUiContext.ShowMessageWindow)); // first of two
        }

        [PatchPrefix]
        public static bool Prefix(ref string description, ref Task<bool> __result)
        {
            if (!InMultiDiscard)
            {
                return true;
            }

            if (!DialogShown)
            {
                StringBuilder sb = new();
                sb.AppendLine("Are you sure you want to destroy".Localized());
                sb.AppendLine();

                int count = 0;
                foreach (Item item in MultiSelect.Items)
                {
                    sb.Append(item.LocalizedName());
                    if (item.StackObjectsCount > 1)
                    {
                        sb.Append($" (x{item.StackObjectsCount})");
                    }

                    sb.AppendLine();

                    if (++count >= 30)
                    {
                        break;
                    }
                }

                // If there are more items than we can show
                if (MultiSelect.Count > count)
                {
                    sb.AppendLine("...");
                    sb.AppendLine($"+ {MultiSelect.Count - count}");
                }

                description = sb.ToString();
                DialogShown = true;

                return true;
            }

            __result = Task.FromResult(Result);
            return false;
        }

        [PatchPostfix]
        public static async void Postfix(Task<bool> __result)
        {
            Result = await __result;
        }
    }
}