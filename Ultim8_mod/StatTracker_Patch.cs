using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class StatTracker_Patch : InventoryBook
	{
		public static void patch()
		{
			MethodInfo StatTrackerInitialize = typeof(StatTracker).GetMethod("Initialize");
			MethodInfo StatTrackerInitializer = typeof(StatTracker_Patch).GetMethod("StatTrackerInitialize");
			Debug.Log("StatTrackerInitialize " + StatTrackerInitialize + " StatTrackerInitializer " + StatTrackerInitializer);
			new Detour(StatTrackerInitialize, StatTrackerInitializer);

			MethodInfo GetSaveFileDataForLocalPlayer = typeof(StatTracker).GetMethod("GetSaveFileDataForLocalPlayer");
			MethodInfo GetSaveFileDataForLocalPlayerr = typeof(StatTracker_Patch).GetMethod("GetSaveFileDataForLocalPlayer");
			Debug.Log("GetSaveFileDataForLocalPlayer " + GetSaveFileDataForLocalPlayer + " GetSaveFileDataForLocalPlayerr " + GetSaveFileDataForLocalPlayerr);
			new Detour(GetSaveFileDataForLocalPlayer, GetSaveFileDataForLocalPlayerr);

			MethodInfo OnLocalPlayerAdded = typeof(StatTracker).GetMethod("OnLocalPlayerAdded");
			MethodInfo OnLocalPlayerAddedr = typeof(StatTracker_Patch).GetMethod("OnLocalPlayerAdded");
			Debug.Log("OnLocalPlayerAdded " + OnLocalPlayerAdded + " OnLocalPlayerAddedr " + OnLocalPlayerAddedr);
			Detour dOnLocalPlayerAdded = new Detour(OnLocalPlayerAdded, OnLocalPlayerAddedr);
		}

		public void StatTrackerInitialize()
		{
			Debug.Log("StatTrackerInitialize");

			StatTracker.Instance.saveFiles = new SaveFileData[PlayerManager.maxPlayers];
			StatTracker.Instance.saveStatuses = new StatTracker.SaveFileStatus[PlayerManager.maxPlayers];
		}


		public SaveFileData GetSaveFileDataForLocalPlayer(int localPlayerNumber, bool fallback = false)
		{
			if (localPlayerNumber <= 0 || localPlayerNumber > PlayerManager.maxPlayers)
			{
				return null;
			}
			if (!fallback)
			{
				return StatTracker.Instance.saveFiles[localPlayerNumber - 1];
			}
			if (StatTracker.Instance.saveFiles[localPlayerNumber - 1] == null)
			{
				return StatTracker.Instance.GetSaveFileDataForMainUser();
			}
			return StatTracker.Instance.saveFiles[localPlayerNumber - 1];
		}

		public void OnLocalPlayerAdded(int playerLocalNumber)
		{
			

			if (playerLocalNumber <= 0 || playerLocalNumber > PlayerManager.maxPlayers)
			{
				Debug.LogError("ERROR: Illegal player local number (" + playerLocalNumber.ToString() + ") max " + PlayerManager.maxPlayers);
				return;
			}
			Player player = PlayerManager.GetInstance().GetPlayer(playerLocalNumber);
			if (player == null)
			{
				Debug.LogError("StatTracker.OnLocalPlayerAdded: Could not find player with local number " + playerLocalNumber.ToString());
				return;
			}
			if (ControllerMonitor.Instance.IsMainController(player.UseController))
			{
				StatTracker.Instance.saveFiles[playerLocalNumber - 1] = StatTracker.Instance.mainUserSaveFileData;
				return;
			}
			if (StatTracker.Instance.PlatformHasMultiSave)
			{
				SaveFileData saveFileData = new SaveFileData();
				StatTracker.Instance.saveFiles[playerLocalNumber - 1] = saveFileData;
				StatTracker.Instance.saveStatuses[playerLocalNumber - 1] = StatTracker.SaveFileStatus.NONE;
				StatTracker.Instance.LoadGameForUser(playerLocalNumber, saveFileData);
				return;
			}
			StatTracker.Instance.saveFiles[playerLocalNumber - 1] = null;
		}

	}
}
