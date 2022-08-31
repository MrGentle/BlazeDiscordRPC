using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LLBML;
using UnityEngine;
using Discord;

namespace BlazeDiscordRPC {
	public class GameIntegration : MonoBehaviour {

		private Activity lastActivity;

		void Awake() {
			LLBML.GameEvents.GameStateEvents.OnStateChange += HandleGameStateEvent;
			LLBML.GameEvents.LobbyEvents.OnLobbyEntered += HandleLobbyEnteredEvent;
			LLBML.GameEvents.LobbyEvents.OnLobbyReady += HandleLobbyReadyEvent;
		}

		public void HandleGameStateEvent(object sender, LLBML.GameEvents.OnStateChangeArgs args) {
			BlazeDiscordRPC.Log.LogInfo($"{args.newState}");

			Activity activity = new Activity ();

			OnlineMode onlineMode = JOMBNFKIHIC.EAENFOJNNGP;
			JOMBNFKIHIC game = JOMBNFKIHIC.GIGAKBJGFDI;

            
			string icon = "icon";
            string largeText = "";
			bool updateActivity = true;
			bool playing = false;
			string character = "";

			if (game.OOEPDFABFIP != 0) {
                icon = game.OOEPDFABFIP.ToString();
                largeText = "Map: " + icon;
            }

			switch (args.newState) {
				case var value when value == LLBML.States.GameState.MENU: activity.State = "In Main Menu"; break;

				case var value when value == LLBML.States.GameState.STORY_GRID: activity.State = "In Story Selection"; break;
				case var value when value == LLBML.States.GameState.STORY_COMIC: activity.State = "In Story Comic"; break;

				case var value when value == LLBML.States.GameState.LOBBY_STORY: activity.State = "Character Select: Story"; break;
				case var value when value == LLBML.States.GameState.LOBBY_CHALLENGE: activity.State = "Character Select: Arcade"; break;
				case var value when value == LLBML.States.GameState.LOBBY_LOCAL: activity.State = "In Local Lobby"; break;
				case var value when value == LLBML.States.GameState.LOBBY_TRAINING: activity.State = "Character Select: Training"; break;
				case var value when value == LLBML.States.GameState.LOBBY_TUTORIAL: activity.State = "Character Select: Tutorial"; break;
					

				case var value when value == LLBML.States.GameState.LOBBY_ONLINE:
					switch (onlineMode) {
						case OnlineMode.QUICKMATCH: activity.State = "Searching for Quick Match"; break;
						case OnlineMode.RANKED: activity.State = "Searching for Ranked Match"; break;
						case OnlineMode.HOSTED: activity.State = "In Private Lobby"; break;
					}
					break;

				
				case var value when value == LLBML.States.GameState.GAME_INTRO:
					switch (args.oldState) {
						case var lobby when lobby == LLBML.States.GameState.LOBBY_CHALLENGE: activity.State = "In Arcade Mode"; break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_LOCAL: activity.State = "In Local Match"; break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_ONLINE:
							switch (onlineMode) {
								case OnlineMode.QUICKMATCH: activity.State = "In Quick Match"; break;
								case OnlineMode.RANKED: activity.State = "In Ranked Match"; break;
								case OnlineMode.HOSTED: activity.State = "In Private Match"; break;
							}
							break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_STORY: activity.State = "In Story Mode"; break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_TRAINING: activity.State = "In Training Mode"; break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_TUTORIAL: activity.State = "In Tutorial"; break;
					}
					break;

				case var value when value == LLBML.States.GameState.GAME:
					activity = lastActivity;
					character = LLBML.Utils.StringUtils.characterNames[LLBML.Players.Player.GetLocalPlayer().Character];
					playing = true;
					break;

				default:
					updateActivity = false;
					break;
			}

			if (playing) {
                activity.Timestamps = new ActivityTimestamps() { Start = 0 };
                activity.Assets.LargeImage = icon.ToLower();
                activity.Assets.LargeText = largeText;
            } else {
                activity.Details = "";
                activity.Timestamps = new ActivityTimestamps();
                activity.Assets.LargeImage = "icon";
                activity.Assets.LargeText = "Lethal League Blaze";
            }

            //Debug.Log(chara);
            if (character.Length > 0 && playing) {
                activity.Assets.SmallImage = CharacterApi.GetCharacterByName(character).ToString();
                activity.Assets.SmallText = "Playing " + character;
            } else {
                activity.Assets.SmallImage = "";
                activity.Assets.SmallText = "";
            }

			lastActivity = activity;
			DiscordInterface.SetActivity(activity);
		}

		public void HandleLobbyEnteredEvent(object sender, LLBML.GameEvents.LobbyEventArgs args) {
			BlazeDiscordRPC.Log.LogInfo($"Host id: {args.host_id} Lobby id: {args.lobby_id}");
		}

		public void HandleLobbyReadyEvent(object sender, LLBML.GameEvents.OnLobbyReadyArgs args) {
			BlazeDiscordRPC.Log.LogInfo($"Host id: {args.host_id} Lobby id: {args.lobby_id}");
		}
	}
}
