using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;
using Discord;
using System.Diagnostics;
using System.IO;


namespace BlazeDiscordRPC {
	static class DiscordInterface {
        public static bool discordInitialized = false;
        public static Discord.Discord discord = null;
        public static ActivityManager activityManager = null;

        public static void StartDiscord() {
            if (CopyDiscordSDKToGameFolder()) {
                InitializeDRPC();
			}
		}

        static Activity activity = new Activity {
            Details = "R2MM LLBlaze Discord Rich presence",
            State = getLaunchMessage(),
        };


		static bool CopyDiscordSDKToGameFolder() {
            //If we don't have this file in the root of the game folder, Discord RPC will crash
            if (!File.Exists(Path.Combine(Paths.GameRootPath, "discord_game_sdk.dll"))) {
                try {
                    File.Copy(Path.Combine(Directory.GetParent(BlazeDiscordRPC.pluginInfo.Location).FullName, "discord_game_sdk"), Path.Combine(Paths.GameRootPath, "discord_game_sdk.dll"));
                    return true;
                } catch (Exception ex) {
                    BlazeDiscordRPC.Log.LogFatal($"Couldn't copy the Discord SDK to the game folder, please do so manually. You can find the file in [{Directory.GetParent(BlazeDiscordRPC.pluginInfo.Location).FullName}], copy it to [{Paths.GameRootPath}/discord_game_sdk.dll]");
                    return false;
				}
			}

            return true;
		}

        static void InitializeDRPC() {
            //If Discord isn't running, we don't want to provide Rich Presence
            //This is to circumvent an issue where steam starts Discord instead of LLB
            Process[] processes = Process.GetProcessesByName("discord");
            if (processes.Length > 0) {
                discord = new Discord.Discord(1014254157353455656, (UInt64)CreateFlags.Default);
                activityManager = discord.GetActivityManager();
                activityManager.RegisterSteam(553310);
                activity.Assets.LargeImage = "logo1";
                activityManager.UpdateActivity(activity, (res) => {});
                discordInitialized = true;
			} else {
                BlazeDiscordRPC.Log.LogFatal("Discord is not running: Can't provide rich presence");
			}
		}

        
        public static void SetActivity(Activity activity) {
            activityManager.UpdateActivity(activity, (res) => {
                if (res != Result.Ok) {
                    BlazeDiscordRPC.Log.LogError($"Failed updating rich presence [{res}]");
                }
            });
		}

        private static string getLaunchMessage() {
            List<string> quotes = new() {
                "- Blaze initializing",
                "- Loading mods",
                "- A new challenger approaches",
                "- Loading blazingly fast",
                "- Lethal loading",
                "- Time to grind!",
                "- About to ball!",
                "- Putting on anti-gravity pants",
                "- Loading Elevator...",
                "- * Doombox noises *",
                "- Watching Intro",
                "- Revving up the game engine",
                "- * Dial-up noises *",
                "- Injecting skills",
                "- Skipping intro",
                "- Putting on the Blazer",
                "- Slapping that like button",
                "- * Checks rich presence *",
                "- Cranking the knob to 11",
                "- Hamster winding up",
                "- Local Blaze players near you",
                "- Let's roll!",
                "- Ez prei",
                "- * Spamming smash parry *",
                "- Locked in deadframes",
                "- Host advantage!!",
                "- Speedrunning the loading screen",
                "- @LFG",
                "- Surfing the information highway",
                "- Checking BlazeLB",
                "- WITNESS!",
                "- Ready to fight",
                "- Loading fresh beats",
                "- Stretching hands",
                "- Cracking knuckles",
                "- Brandishing Candycane",
                "- Testing speakers",
                "- Roaming the streets",
                "- Stearing at intro screen",
                "- Watching bots play",
                "- Get it on the ThunderStore",
                "- Resetting timer",
                "- Contemplating life choices",
                "- Axelotl is my king",
                "- just after these messages"
			};

            return quotes[(new System.Random()).Next(quotes.Count)];

		}
	}
}
