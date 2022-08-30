using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;

namespace BlazeDiscordRPC
{
    [BepInPlugin("no.gentle.plugin.blazediscordrpc", "BlazeDiscordRPC", "0.0.1")]
    public class SeasonFetcher : BaseUnityPlugin {
        internal static SeasonFetcher Instance { get; private set; } = null;
        internal static ManualLogSource Log { get; private set; } = null;
        public static Harmony harmony = new(PluginInfo.PLUGIN_NAME);

        private void Awake() {
            Instance = this;
            Log = Logger;
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            //harmony.PatchAll(typeof(Patches.));
        }
    }
}
