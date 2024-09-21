using BepInEx;

namespace Discarder
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("Tyfon.UIFixes", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            ContextMenuPatches.Enable();
            DiscardPatches.Enable();
        }
    }
}
