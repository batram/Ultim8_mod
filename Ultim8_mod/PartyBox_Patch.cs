using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class PartyBox_Patch : InventoryBook
	{
        public static void patch()
        {

            //PartyBox.SetPlayerCount
            MethodInfo SetPlayerCount = typeof(PartyBox).GetMethod("SetPlayerCount", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo SetPlayerCountr = typeof(PartyBox_Patch).GetMethod("SetPlayerCount", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Debug.Log("SetPlayerCount " + SetPlayerCount + " SetPlayerCountr " + SetPlayerCountr);
            new Detour(SetPlayerCount, SetPlayerCountr);

            //PartyBox.AddPlayer
            MethodInfo AddPlayer = typeof(PartyBox).GetMethod("AddPlayer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo AddPlayerr = typeof(PartyBox_Patch).GetMethod("AddPlayerx", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Debug.Log("AddPlayer " + AddPlayer + " AddPlayerr " + AddPlayerr);
            new Detour(AddPlayer, AddPlayerr);
        }

		/* function PartyBox.AddPlayer	hardcoded 4 comparison */
        public PartyPickCursor AddPlayerx(int playerNumber, Character.Animals animal)
		{
			Debug.Log("PartyPickCursor AddPlayerx");
			if (playerNumber > PlayerManager.maxPlayers || playerNumber < 1)
			{
				return null;
			}

			var prop = this.GetType().GetField("CursorPrefab", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var CursorPrefab = prop.GetValue(this) as PartyPickCursor;

			Debug.Log("CursorPrefab" + CursorPrefab);

			var prop2 = this.GetType().GetField("UICamera", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var UICamera = prop2.GetValue(this) as Camera;

			var prop4 = this.GetType().GetField("cursors", BindingFlags.NonPublic | BindingFlags.Instance);
			var cursors = prop4.GetValue(this) as PartyPickCursor[];
			Debug.Log("PartyPickCursor AddPlayerx " + cursors.Length);

			PartyPickCursor partyPickCursor = UnityEngine.Object.Instantiate<PartyPickCursor>(CursorPrefab);
			partyPickCursor.name = animal.ToString() + " party cursor";
			AkSoundEngine.SetSwitch("Character", animal.ToString(), base.gameObject);
			GamePlayer gamePlayer = LobbyManager.instance.PlayerTracker.GetGamePlayer(playerNumber);
			partyPickCursor.NetworkPlayerAnimal = animal;
			partyPickCursor.NetworkFindControllerOnSpawn = true;
			partyPickCursor.NetworklocalNumber = gamePlayer.localNumber;
			partyPickCursor.LocalPlayer = gamePlayer.LocalPlayer;
			partyPickCursor.NetworknetworkNumber = gamePlayer.networkNumber;
			partyPickCursor.AssociatedGamePlayer = gamePlayer;
			partyPickCursor.SetLayer(5, true);
			partyPickCursor.UseCamera = UICamera;
			partyPickCursor.Disable(true, false);
			string[] array = new string[8];
			array[0] = "[Net] ";
			int num = 1;
			PartyPickCursor partyPickCursor2 = partyPickCursor;
			array[num] = ((partyPickCursor2 != null) ? partyPickCursor2.ToString() : null);
			array[2] = " - ";
			int num2 = 3;
			LobbyManager instance = LobbyManager.instance;
			array[num2] = ((instance != null) ? instance.ToString() : null);
			array[4] = " - ";
			int num3 = 5;
			GamePlayer gamePlayer2 = gamePlayer;
			array[num3] = ((gamePlayer2 != null) ? gamePlayer2.ToString() : null);
			array[6] = " - ";
			array[7] = playerNumber.ToString();
			Debug.Log(string.Concat(array));
			UnityEngine.Networking.NetworkServer.SpawnWithClientAuthority(partyPickCursor.gameObject, gamePlayer.gameObject);
			if (cursors.Length >= playerNumber)
			{
				cursors[playerNumber - 1] = partyPickCursor;
			}
			return partyPickCursor;
		}

		/* function PartyBox.SetPlayerCount	hardcoded 4 comparison */
		public void SetPlayerCount(int players)
		{
			if (players < 1 || players > PlayerManager.maxPlayers)
			{
				Debug.LogError("PartyBox.SetPlayerCount: invalid number of players: " + players.ToString());
				return;
			}

			var prop = this.GetType().GetField("cursors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			prop.SetValue(this, new PartyPickCursor[players]);
		}
	}
}
