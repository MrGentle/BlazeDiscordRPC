using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using Discord;
using BepInEx.Configuration;
using System.Collections.Generic;

namespace BlazeDiscordRPC
{
    [BepInPlugin("no.gentle.plugin.blazediscordrpc", "BlazeDiscordRPC", "1.0.0")]
    [BepInDependency(LLBML.PluginInfos.PLUGIN_ID)]
    [BepInDependency("no.mrgentle.plugins.llb.modmenu", BepInDependency.DependencyFlags.SoftDependency)]
    public class BlazeDiscordRPC : BaseUnityPlugin {
        internal static BlazeDiscordRPC Instance { get; private set; } = null;
        internal static ManualLogSource Log { get; private set; } = null;
        internal static ConfigFile ConfigInstance { get; private set; } = null;
        public static Harmony harmony = new(PluginInfo.PLUGIN_NAME);
        public static BepInEx.PluginInfo pluginInfo;
        
        public ConfigEntry<bool> configBlazeLBUpdates;

        private void Awake() {
            Instance = this;
            Log = Logger;
            pluginInfo = Info;
            ConfigInstance = Config;
            
            configBlazeLBUpdates = Config.Bind("BlazeLB", "Send Updates", true, "Enables the functionality that posts data to BlazeLB");

            LLBML.Utils.ModDependenciesUtils.RegisterToModMenu(Info, new List<string>{
                "Blaze Discord RPC interfaces with Discord to give Lethal League Blaze Discord Rich Presence support.","",
                "Rich Presence updates are also posted to BlazeLB to more reliably show who is online in ranked."
            });

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
            DiscordInterface.StartDiscord();

            if (DiscordInterface.discordInitialized) { 
                Instance.gameObject.AddComponent<GameIntegration>();
                InvokeRepeating("runCallbacks", 0.1f, 0.1f);
            }
        }

        private void runCallbacks() {
            DiscordInterface.discord.RunCallbacks();
		}

        void OnDestroy() {
            Log.LogInfo("Clearing discord status....");
            DiscordInterface.activityManager.ClearActivity((res) => {
                if (res == Result.Ok) Log.LogInfo("Cleared discord status on exit");
                else Log.LogError(res);
            });
            DiscordInterface.discord.Dispose();
        }
    }
}
