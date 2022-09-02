using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using Discord;


namespace BlazeDiscordRPC
{
    [BepInPlugin("no.gentle.plugin.blazediscordrpc", "BlazeDiscordRPC", "0.0.4")]
    [BepInDependency(LLBML.PluginInfos.PLUGIN_ID)]
    public class BlazeDiscordRPC : BaseUnityPlugin {
        internal static BlazeDiscordRPC Instance { get; private set; } = null;
        internal static ManualLogSource Log { get; private set; } = null;
        public static Harmony harmony = new(PluginInfo.PLUGIN_NAME);
        public static BepInEx.PluginInfo pluginInfo;
        
        private void Awake() {
            Instance = this;
            Log = Logger;
            pluginInfo = Info;
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
            DiscordInterface.StartDiscord();

            if (DiscordInterface.discordInitialized) { 
                Instance.gameObject.AddComponent<GameIntegration>();
                InvokeRepeating("runCallbacks", 1, 1);
            }
        }

        void OnDestroy() {
            Log.LogInfo("Clearing discord status....");
            DiscordInterface.activityManager.ClearActivity((res) => {
                if (res == Result.Ok) Log.LogInfo("Cleared discord status on exit");
                else Log.LogError(res);
            });
            DiscordInterface.discord.Dispose();
        }


        private void runCallbacks() {
            DiscordInterface.discord.RunCallbacks();
		}
    }
}
