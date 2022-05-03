using GameEvent;
using MonoMod.RuntimeDetour;
using Moserware.Skills;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Moserware.Skills.TrueSkill;

namespace Ultim8_mod
{
    class LobbySkillTracker_Patch : LobbySkillTracker
	{
		public static void patch()
		{
			//LobbySkillTracker.Start
			MethodInfo a = typeof(LobbySkillTracker).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			MethodInfo b = typeof(LobbySkillTracker_Patch).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			//LobbySkillTracker.RecalculateScores
			a = typeof(LobbySkillTracker).GetMethod("RecalculateScores", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			b = typeof(LobbySkillTracker_Patch).GetMethod("RecalculateScores", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			//UpdateLobbyInfo for (int num2 = 0; num2 != 4; num2++)
		}

		/* class LobbySkillTracker
			ratings = new Rating[4];
		*/
		public void Start()
		{
			var prop = this.GetType().GetField("ratings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			prop.SetValue(this, new Moserware.Skills.Rating[PlayerManager.maxPlayers]);
			GameEventManager.ChangeListener<LobbyPlayerCreatedEvent>(this, true);
			GameEventManager.ChangeListener<LobbyPlayerRemovedEvent>(this, true);
			//GameEventManager.ChangeListener<GameResultsEvent>(this, true);
		}

		/* function LobbySkillTracker.RecalculateScores hardcoded 4 comparison */
		new public void RecalculateScores(IDictionary<GamePlayer, int> gameResults)
		{
			if (gameResults.Count <= 1)
			{
				return;
			}
			// TODO: maybe fixup
			this.UpdateLobbyInfo();
		}
	}
}
