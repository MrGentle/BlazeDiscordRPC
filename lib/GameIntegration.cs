using System;
using UnityEngine;
using Discord;
using LLBML.Players;

namespace BlazeDiscordRPC {
	public class GameIntegration : MonoBehaviour {

		private Activity activity = new Activity();
		string status = "";

		void Awake() {
			LLBML.GameEvents.GameStateEvents.OnStateChange += HandleGameStateEvent;
		}

		public void HandleGameStateEvent(object sender, LLBML.GameEvents.OnStateChangeArgs args) {
			BlazeDiscordRPC.Log.LogInfo($"{args.newState}");

			OnlineMode onlineMode = JOMBNFKIHIC.EAENFOJNNGP;
			JOMBNFKIHIC game = JOMBNFKIHIC.GIGAKBJGFDI;

			string details = "";
			long startTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
			bool updateActivity = true;
			bool playing = false;
			string characterName = "";
			Character character = Character.NONE;


			switch (args.newState) {
				case var value when value == LLBML.States.GameState.MENU: status = "In Main Menu"; break;
				case var value when value == LLBML.States.GameState.UNLOCKS: status = "Browsing Collection"; break;

				case var value when value == LLBML.States.GameState.STORY_GRID: status = "In Story Selection"; break;
				case var value when value == LLBML.States.GameState.STORY_COMIC: status = "In Story Comic"; break;

				case var value when value == LLBML.States.GameState.LOBBY_STORY: status = "Character Select: Story"; break;
				case var value when value == LLBML.States.GameState.LOBBY_CHALLENGE: status = "Character Select: Arcade"; break;
				case var value when value == LLBML.States.GameState.LOBBY_LOCAL: status = "In Local Lobby"; break;
				case var value when value == LLBML.States.GameState.LOBBY_TRAINING: status = "Character Select: Training"; break;
				case var value when value == LLBML.States.GameState.LOBBY_TUTORIAL: status = "Character Select: Tutorial"; break;
					

				case var value when value == LLBML.States.GameState.LOBBY_ONLINE:
					switch (onlineMode) {
						case OnlineMode.QUICKMATCH: status = "Searching for Quick Match"; break;
						case OnlineMode.RANKED: status = "Searching for Ranked Match"; break;
						case OnlineMode.HOSTED: status = "In Private Lobby"; break;
					}
					break;



				case var value when value == LLBML.States.GameState.GAME_INTRO:
					switch (args.oldState) {
						case var lobby when lobby == LLBML.States.GameState.CHALLENGE_LADDER: status = "In Arcade Mode"; break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_LOCAL: status = "In Local Match"; break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_ONLINE:
							switch (onlineMode) {
								case OnlineMode.QUICKMATCH: status = "In Quick Match"; break;
								case OnlineMode.RANKED: status = "In Ranked Match"; break;
								case OnlineMode.HOSTED: status = "In Private Match"; break;
							}
							break;
						case var lobby when lobby == LLBML.States.GameState.STORY_COMIC: status = "In Story Mode"; break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_TRAINING: status = "In Training Mode"; break;
						case var lobby when lobby == LLBML.States.GameState.LOBBY_TUTORIAL: status = "In Tutorial"; break;
					}
					break;

				case var value when value == LLBML.States.GameState.GAME:
					character = Player.GetLocalPlayer().Character;
					characterName = character == Character.ELECTRO ? "Grid" : LLBML.Utils.StringUtils.characterNames[character];
					playing = true;
					break;
				
				case var value when value == LLBML.States.GameState.QUIT:
					StartCoroutine(BlazeLBInterface.UpdateStatus(new Activity(), "false"));
					updateActivity = false;
					break;

				default:
					updateActivity = false;
					break;
			}

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
				}

				activity.State = status;
				activity.Details = details;

				StartCoroutine(BlazeLBInterface.UpdateStatus(activity, "true"));
				DiscordInterface.SetActivity(activity);
			}
		}
	}
}
