using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class GraphScoreBoard_Patch : GraphScoreBoard
	{
        static public void patch()
		{
			//GraphScoreBoard.SetPlayerCount
			MethodInfo GraphScoreBoardSetPlayerCount = typeof(GraphScoreBoard).GetMethod("SetPlayerCount", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo GraphScoreBoardSetPlayerCountr = typeof(GraphScoreBoard_Patch).GetMethod("SetPlayerCount", BindingFlags.Instance | BindingFlags.Public);
			new Detour(GraphScoreBoardSetPlayerCount, GraphScoreBoardSetPlayerCountr);

			//GraphScoreBoard.SetPlayerCharacter
			MethodInfo a = typeof(GraphScoreBoard).GetMethod("SetPlayerCharacter", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo b = typeof(GraphScoreBoard_Patch).GetMethod("SetPlayerCharacter", BindingFlags.Instance | BindingFlags.Public);
			new Detour(a, b);
		}

		/* function GraphScoreBoard.SetPlayerCount hardcoded 4 comparison, add ScorePositions for additional players */
		new public void SetPlayerCount(int numberPlayers)
		{			

			if (numberPlayers == 0 || numberPlayers > PlayerManager.maxPlayers)
			{
				Debug.LogError("GraphScoreBoard.SetPlayerCount: invalid number of players: " + numberPlayers.ToString());
			}
			this.playerScoreLines = new ScoreLine[numberPlayers];

			Debug.LogError("GraphScoreBoard.SetPlayerCount");
			Vector3 vector = this.ScorePositions[0].position + new Vector3(0f, 1.25f, 0f);
			for (int num = 0; num != numberPlayers; num++)
			{
				/* add ScorePositions for additional players */
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.scoreLinePrefab.gameObject, vector - new Vector3(0f, (float)num * 1.25f, 0f), Quaternion.identity);
				gameObject.transform.SetParent(this.mainParent);
				gameObject.transform.localScale = new Vector3(1f, 0.5f, 1f);
				playerScoreLines[num] = gameObject.GetComponent<ScoreLine>();
				playerScoreLines[num].scoreBoardParent = this;
			}
		}

		/* function GraphScoreBoard.SetPlayerCharacter	hardcoded 3 comparison (order: 0 to 3) */
		new public void SetPlayerCharacter(int order, Character.Animals character, bool altSkin, LobbyPlayer lobbyPl, int handicap)
		{
			if (order < 0 || order > PlayerManager.maxPlayers - 1)
			{
				return;
			}
			ScoreLine scoreLine = this.playerScoreLines[order];
			scoreLine.Animal = character;
			scoreLine.animalImage.sprite = CharacterSpriteManager.GetInstance().GetCharaterAliveIcon(character);
			scoreLine.SetHandicap(handicap);
			if (!LobbyManager.instance.IsInOnlineGame)
			{
				scoreLine.UseAnimalName(Character.GetLocalizedAnimal(character, altSkin));
			}
			else
			{
				scoreLine.UsePlayerName(lobbyPl);
			}

			var prop = this.GetType().GetField("scorelineRelation", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var scorelineRelation = prop.GetValue(this) as Dictionary<int, ScoreLine>;


			if (!scorelineRelation.ContainsKey(lobbyPl.networkNumber))
			{
				scorelineRelation.Add(lobbyPl.networkNumber, scoreLine);
				return;
			}
			Debug.Log("Player number " + lobbyPl.networkNumber.ToString() + " already exists in scoreboard");
		}
	}
}
