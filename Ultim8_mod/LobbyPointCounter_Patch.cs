using GameEvent;
using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class LobbyPointCounter_Patch : LobbyPointCounter
	{
		public static void patch()
		{
			MethodInfo a = typeof(LobbyPointCounter).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo b = typeof(LobbyPointCounter_Patch).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);
		}

		/*	class LobbyPointCounter 
			playerJoinedGame
			playerPlayedGame
			playerAFK
		*/
		private void Start()
		{
			var prop2 = this.GetType().GetField("playerJoinedGame", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			prop2.SetValue(this, new bool[PlayerManager.maxPlayers]);
			var prop4 = this.GetType().GetField("playerPlayedGame", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			prop4.SetValue(this, new bool[PlayerManager.maxPlayers]);
			var prop5 = this.GetType().GetField("playerAFK", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			prop5.SetValue(this, new bool[PlayerManager.maxPlayers]);

			var prop3 = this.GetType().GetField("inLobby", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			prop3.SetValue(this, true);

			var adding = true;
			GameEventManager.ChangeListener<NetworkPlayerConnectEvent>(this, adding);
			GameEventManager.ChangeListener<NetworkPlayerDisconnectEvent>(this, adding);
			GameEventManager.ChangeListener<GameStartEvent>(this, adding);
			GameEventManager.ChangeListener<GameEndEvent>(this, adding);
			GameEventManager.ChangeListener<NetworkMessageReceivedEvent>(this, adding);
			GameEventManager.ChangeListener<RoundCompleteEvent>(this, adding);
		
			this.Reset();
		}

	}


}
