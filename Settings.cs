using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace Discarder
{
    public class Settings
    {
        // Categories
        private const string GeneralSection = "1. General";

        // General

        public static void Init(ConfigFile config)
        {
            var configEntries = new List<ConfigEntryBase>();

            // General

            RecalcOrder(configEntries);
        }
        private static void RecalcOrder(List<ConfigEntryBase> configEntries)
        {
            // Set the Order field for all settings, to avoid unnecessary changes when adding new settings
            int settingOrder = configEntries.Count;
            foreach (var entry in configEntries)
            {
                if (entry.Description.Tags[0] is ConfigurationManagerAttributes attributes)
                {
                    attributes.Order = settingOrder;
                }

                settingOrder--;
            }
        }
    }
}
