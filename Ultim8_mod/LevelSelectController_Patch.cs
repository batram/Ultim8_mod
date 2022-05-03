using GameEvent;
using MonoMod.RuntimeDetour;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class LevelSelectController_Patch : LevelSelectController
	{
		public static void patch()
        {
			//LevelSelectController.ChangeListener
			MethodInfo a = typeof(LevelSelectController).GetMethod("ChangeListener", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			MethodInfo b = typeof(LevelSelectController_Patch).GetMethod("ChangeListener", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);
		}

		/* class LevelSelectController
			JoinedPlayers = new LobbyPlayer[4]
			PlayerJoinIndicators	map indicators to 0..3, we reuse existing 4 indicators for multiple players 
			CursorSpawnPoint		map cursor spawn points to 0..3
		*/
		new public void ChangeListener(bool adding)
		{
			GameEventManager.ChangeListener<LobbyPlayerRemovedEvent>(this, adding);
			GameEventManager.ChangeListener<LocalPlayerRemovedEvent>(this, adding);
			GameEventManager.ChangeListener<CharacterPickedEvent>(this, adding);
			GameEventManager.ChangeListener<CharacterVoteEvent>(this, adding);
			GameEventManager.ChangeListener<LobbyPlayerCreatedEvent>(this, adding);
			GameEventManager.ChangeListener<LobbyCursorCreatedEvent>(this, adding);
			GameEventManager.ChangeListener<CheatUnlockEvent>(this, adding);
			GameEventManager.ChangeListener<CheatUnlockHalfEvent>(this, adding);
			GameEventManager.ChangeListener<OneUnlockMaker>(this, adding);
			GameEventManager.ChangeListener<NetworkClientDisconnectEvent>(this, adding);
			GameEventManager.ChangeListener<NetworkMessageReceivedEvent>(this, adding);
			GameEventManager.ChangeListener<GameModeSetEvent>(this, adding);
			GameEventManager.ChangeListener<ResetDataEvent>(this, adding);
			GameEventManager.ChangeListener<DrivingPlayerRemovedEvent>(this, adding);
			GameEventManager.ChangeListener<PlatformPlayerRemovedEvent>(this, adding);
			GameEventManager.ChangeListener<CheatKonamiEvent>(this, adding);

			/* JoinedPlayers = new LobbyPlayer[4] */
			if (this.JoinedPlayers.Length < PlayerManager.maxPlayers)
			{
				Array.Resize<LobbyPlayer>(ref this.JoinedPlayers, PlayerManager.maxPlayers);
			}

			Debug.Log("PlayerJoinIndicators.Length " + this.PlayerJoinIndicators.Length);
			if (this.PlayerJoinIndicators.Length < PlayerManager.maxPlayers)
			{
				int num = this.PlayerJoinIndicators.Length;
				Array.Resize<playerJoinIndicator>(ref this.PlayerJoinIndicators, PlayerManager.maxPlayers);
				for (int i = num; i < this.PlayerJoinIndicators.Length; i++)
				{
					/* map PlayerJoinIndicators to 0..3, we reuse existing 4 indicators for multiple players */
					this.PlayerJoinIndicators[i] = this.PlayerJoinIndicators[i % num];
				}
			}

			Debug.Log("CursorSpawnPoint.Length " + this.CursorSpawnPoint.Length);
			if (this.CursorSpawnPoint.Length < PlayerManager.maxPlayers)
			{
				int num = this.CursorSpawnPoint.Length;
				Array.Resize(ref this.CursorSpawnPoint, PlayerManager.maxPlayers);
				for (int i = num; i < this.CursorSpawnPoint.Length; i++)
				{
					/* map CursorSpawnPoint to 0..3
						TODO: add new positions, so cursors aren't hidden behind each other on spawn
					 */
					this.CursorSpawnPoint[i] = this.CursorSpawnPoint[i % num];
				}
			}
		}
	}
}
