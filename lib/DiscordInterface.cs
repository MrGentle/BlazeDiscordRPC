using System;
using System.Collections.Generic;
using BepInEx;
using Discord;
using System.Diagnostics;
using System.IO;
using Steamworks;

namespace BlazeDiscordRPC {
	static class DiscordInterface {
        public static bool discordInitialized = false;
        public static Discord.Discord discord = null;
        public static ActivityManager activityManager = null;
        public static LobbyManager lobbyManager = null;

        public static bool reachedMainMenu = false;

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
                lobbyManager = discord.GetLobbyManager();
                
                activityManager.OnActivityJoinRequest += ActivityJoinRequestHandler;
                activityManager.OnActivityJoin += ActivityJoinHandler;
                
                discordInitialized = true;
			} else {
                BlazeDiscordRPC.Log.LogFatal("Discord is not running; Can't provide rich presence");
			}
		}

        static void ActivityJoinRequestHandler(ref User user) { //Fires when a user asks to join the current user's game.
            BlazeDiscordRPC.Log.LogDebug($"ActivityJoinRequestHandler {user}");

            activityManager.SendRequestReply(user.Id, ActivityJoinRequestReply.Yes, (res) => {
                BlazeDiscordRPC.Log.LogDebug("ActivityJoinRequest RES: " + res.ToString());
            });
		}

        static void ActivityJoinHandler(string secret) { //Fires when a user accepts a game chat invite or receives confirmation from Asking to Join.
            if (!reachedMainMenu) KKMGLMJABKH.LDJEKOMHIAC(secret);
            else {
                KKMGLMJABKH.JKGMFAGOINC = new CSteamID(ulong.Parse(secret));
                LLBML.States.GameStates.Send(Msg.IR_JOIN_INVITE, -1, -1);
			}
		}

        private struct InviteData {
            public ActivityActionType type;
            public User user;
            public Activity activity;

            public InviteData(ActivityActionType type, User user, Activity activity) {
                this.type = type;
                this.user = user;
                this.activity = activity;
			}
        }

        
        public static void SetActivity(Activity activity) {
            activityManager.UpdateActivity(activity, (res) => {
                if (res != Result.Ok) {
                    BlazeDiscordRPC.Log.LogError($"Failed updating rich presence [{res}]");
                } else BlazeDiscordRPC.Log.LogDebug("Activity updated");
            });
		}

        private static string getLaunchMessage() {
            List<string> quotes = new() {
                "blaze initializing",
                "loading mods",
                "a new challenger approaches",
                "loading blazingly fast",
                "lethal loading",
                "time to grind!",
                "about to ball!",
                "putting on anti-gravity pants",
                "loading Elevator...",
                "* Doombox noises *",
                "watching Intro",
                "revving up the game engine",
                "* Dial-up noises *",
                "injecting skills",
                "skipping intro",
                "putting on the Blazer",
                "slapping that like button",
                "checks rich presence",
                "cranking the knob to 11",
                "hamster winding up",
                "local Blaze players near you",
                "let's roll!",
                "ez prei",
                "* smash parry *",
                "locked in deadframes",
                "host advantage!!",
                "speedrunning the loading screen",
                "@LFG",
                "surfing the information highway",
                "checking BlazeLB",
                "WITNESS!",
                "ready to fight",
                "loading fresh beats",
                "stretching hands",
                "cracking knuckles",
                "brandishing candycane",
                "testing speakers",
                "roaming the streets",
                "stearing at intro screen",
                "watching bots play",
                "get it on the ThunderStore",
                "resetting timer",
                "contemplating life choices",
                "axolotl is my king",
                "just after these messages",
                "praying to our saviour Glom",
                "bending a spoon",
                "churning butter",
                "minting NFTs",
                "can I call you back soon",
                "MOM TOILET",
                "disguising Slayer",
                "pinging imt",
                "muting #create",
                "witty status message",
                "totally not loading",
                "2TB of latch skins",
                "we just don't know",
                "parking cybertruck",
                "renting out the subway",
                "tagging the train",
                "BRC - Behind Release sChedule",
                "fueling stadium flamethrowers",
                "assembling mombox",
                "rolling latchball",
                "did you know?",
                "dice drives a red corolla"
			};

            return "... " + quotes[(new System.Random()).Next(quotes.Count)];

		}
	}
}
