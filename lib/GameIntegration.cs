using System;
using UnityEngine;
using Discord;
using LLBML.Players;
using LLBML.States;

namespace BlazeDiscordRPC {
	public class GameIntegration : MonoBehaviour {

		private Activity activity = new Activity();
		string status = "";
		string details = "";

		void OnApplicationQuit() {
			StartCoroutine(BlazeLBInterface.UpdateStatus(new Activity(), "false"));
		}

		void Awake() {
			LLBML.GameEvents.GameStateEvents.OnStateChange += HandleGameStateEvent;
			LLBML.GameEvents.LobbyEvents.OnLobbyReady += HandleLobbyReadyEvents;
			LLBML.GameEvents.LobbyEvents.OnPlayerJoin += HandlePlayerJoinEvents;
			LLBML.GameEvents.LobbyEvents.OnUnlinkFromPlayer += HandlePlayerLeaveEvents;
			
			InvokeRepeating("CheckForChangedLobbyId", 0.5f, 0.5f); //Blame TR for this mess
		}

		void CheckForChangedLobbyId() { //Blame TR for this mess
			if (activity.Secrets.Join != null && KKMGLMJABKH.KONAAMMBHHG.m_SteamID != 0 && activity.Secrets.Join != KKMGLMJABKH.KONAAMMBHHG.m_SteamID.ToString()) {
				activity.Secrets.Join = KKMGLMJABKH.KONAAMMBHHG.m_SteamID.ToString();
				DiscordInterface.SetActivity(activity);
			}
		}

		public void HandleGameStateEvent(object sender, LLBML.GameEvents.OnStateChangeArgs args) {
			OnlineMode onlineMode = JOMBNFKIHIC.EAENFOJNNGP;
			JOMBNFKIHIC game = JOMBNFKIHIC.GIGAKBJGFDI;

			long startTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
			bool updateActivity = true;
			bool playing = false;
			string characterName = "";
			Character character = Character.NONE;


			switch ((GameState.Enum)args.newState) {
				case GameState.Enum.MENU:
					status = "In Main Menu";
					DiscordInterface.reachedMainMenu = true;
					break;

				case GameState.Enum.UNLOCKS: status = "Browsing Collection"; break;
				case GameState.Enum.STORY_GRID: status = "In Story Selection"; break;
				case GameState.Enum.STORY_COMIC: status = "In Story Comic"; break;

				case GameState.Enum.LOBBY_STORY: status = "Character Select: Story"; break;
				case GameState.Enum.LOBBY_CHALLENGE: status = "Character Select: Arcade"; break;
				case GameState.Enum.LOBBY_LOCAL: status = "In Local Lobby"; break;
				case GameState.Enum.LOBBY_TRAINING: status = "Character Select: Training"; break;
				case GameState.Enum.LOBBY_TUTORIAL: status = "Character Select: Tutorial"; break;
				case GameState.Enum.LOBBY_ONLINE:
					switch (onlineMode) {
						case OnlineMode.QUICKMATCH: status = "Searching for Quick Match"; break;
						case OnlineMode.RANKED: status = "Searching for Ranked Match"; break;
						case OnlineMode.HOSTED: status = "In Private Lobby"; break;
					}
					break;


				case GameState.Enum.GAME_INTRO:
					switch ((GameState.Enum)args.oldState) {
						case GameState.Enum.CHALLENGE_LADDER: status = "In Arcade Mode"; break;
						case GameState.Enum.LOBBY_LOCAL: status = "In Local Match"; break;
						case GameState.Enum.LOBBY_ONLINE:
							switch (onlineMode) {
								case OnlineMode.QUICKMATCH: status = "In Quick Match"; break;
								case OnlineMode.RANKED: status = "In Ranked Match"; break;
								case OnlineMode.HOSTED: status = "In Private Match"; break;
							}
							break;
						case GameState.Enum.STORY_COMIC: status = "In Story Mode"; break;
						case GameState.Enum.LOBBY_TRAINING: status = "In Training Mode"; break;
						case GameState.Enum.LOBBY_TUTORIAL: status = "In Tutorial"; break;
					}
					break;

				case GameState.Enum.GAME:
					character = Player.GetLocalPlayer().Character;
					characterName = LLBML.Utils.StringUtils.characterNames[character];
					playing = true;
					break;

				default:
					updateActivity = false;
					break;
			}

			// Clear Party when leaving the lobby
			if (args.oldState == GameState.LOBBY_ONLINE) {
				activity.Party = new ActivityParty();
			}

			// Time to update the discord activity
			if (updateActivity) {
				if (playing) {
					activity.Timestamps = new ActivityTimestamps() { Start = startTime };

					if (characterName.Length > 0) {
						activity.Assets.SmallImage = character.ToString().ToLower();
						activity.Assets.SmallText = "Playing " + characterName;
						details = activity.Assets.SmallText + " ";
					}

					if (game.OOEPDFABFIP != 0) {
						string stage = game.OOEPDFABFIP.ToString().ToLower();
						activity.Assets.LargeImage = stage;
						string formattedStage = $"{char.ToUpper(stage[0])}{stage.Substring(1)}";
						if (stage.EndsWith("_2d")) formattedStage = "Retro " + $"{formattedStage}".Replace("_2d", "");
						activity.Assets.LargeText = $"on {formattedStage}";
						details += activity.Assets.LargeText;
					}
					
				} else {
					activity.Timestamps = new ActivityTimestamps();
					activity.Assets.LargeImage = "logo1";
					activity.Assets.LargeText = "Lethal League Blaze";
					activity.Assets.SmallImage = "";
					activity.Assets.SmallText = "";
					details = "";
				}

				activity.State = status;
				activity.Details = details;

				StartCoroutine(BlazeLBInterface.UpdateStatus(activity, "true"));
				DiscordInterface.SetActivity(activity);
			}
		}


		private void HandleLobbyReadyEvents(object sender, LLBML.GameEvents.LobbyReadyArgs args) {
			OnlineMode onlineMode = JOMBNFKIHIC.EAENFOJNNGP;
			if (onlineMode == OnlineMode.HOSTED) {
				UpdateParty(args.lobby_id, args.host_id, Player.nPlayersInMatch, Player.MAX_PLAYERS);
			}
		}

		private void HandlePlayerJoinEvents(object sender, LLBML.GameEvents.OnPlayerJoinArgs args) {
			UpdatePartySize(Player.nPlayersInMatch);
		}

		private void HandlePlayerLeaveEvents(object sender, LLBML.GameEvents.OnUnlinkFromPlayerArgs args) {
			if (args.playerNr != Player.LocalPlayerNumber) UpdatePartySize(Player.nPlayersInMatch - 1);
		}

		private void UpdateParty(string lobbyId, string hostId, int size, int maxSize ) {
			activity.Secrets.Join = lobbyId;
			activity.Party.Id = hostId;
			activity.Party.Size.CurrentSize = size;
			activity.Party.Size.MaxSize = maxSize;

			DiscordInterface.SetActivity(activity);	
		}

		private void UpdatePartySize(int size) {
			if (activity.Party.Id != null) {
				activity.Party.Size.CurrentSize = size;
				DiscordInterface.SetActivity(activity);	
			}
		}

	}
}
