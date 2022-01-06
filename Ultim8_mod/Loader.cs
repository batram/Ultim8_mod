using GameEvent;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ultim8_mod
{
    public class Loader : IGameEventListener
    {
        static public int newmaxmax = 8;

        static public void Main(string[] args)
        {
            PlayerManager.maxPlayers = newmaxmax;

            SceneManager.activeSceneChanged += ChangedActiveScene;

            var loader = new Loader();
            GameEventManager.ChangeListener<LocalPlayerAddedEvent>(loader, true);
            GameEventManager.ChangeListener<NetworkHostLobbyLoadedEvent>(loader, true);
            GameEventManager.ChangeListener<ResetDataEvent>(loader, true);
            GameEventManager.ChangeListener<ControllerConnectionEvent>(loader, true);
        }
        private static void ChangedActiveScene(Scene current, Scene next)
        {
            if (next.name == "Init")
            {
                Debug.Log("Ultim8_mod: INIT Scene - mod EYERYTHING! (for " + PlayerManager.maxPlayers + " players)");
                patchAll();
            }
            fixup_schmoo();
        }

        void IGameEventListener.handleEvent(GameEvent.GameEvent _)
        {
            fixup_schmoo();
        }

        static public void fixup_schmoo()
        {
            try
            {
                if (PlayerManager.maxPlayers != newmaxmax)
                {
                    Debug.Log("PlayerManager.maxPlayers change found " + PlayerManager.maxPlayers);
                    PlayerManager.maxPlayers = newmaxmax;
                }


                var propx = typeof(PlayerManager).GetField("playerList", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
                var players = propx.GetValue(PlayerManager.GetInstance()) as Player[];
                if (players.Length < PlayerManager.maxPlayers)
                {
                    Array.Resize<Player>(ref players, newmaxmax);
                    propx.SetValue(PlayerManager.GetInstance(), players);
                }


                if (GameSettings.GetInstance())
                {
                    GameSettings.GetInstance().MaxPlayers = PlayerManager.maxPlayers;

                    if (GameSettings.GetInstance().PlayerColors.Length < PlayerManager.maxPlayers)
                    {
                        int num2 = GameSettings.GetInstance().PlayerColors.Length;
                        Array.Resize<Color>(ref GameSettings.GetInstance().PlayerColors, PlayerManager.maxPlayers);
                        Color[] playerColors = GameSettings.GetInstance().PlayerColors;
                        for (int j = num2; j < playerColors.Length; j++)
                        {
                            GameSettings.GetInstance().PlayerColors[j] = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
                        }
                    }
                }

                if (LobbyManager.instance != null && LobbyManager.instance.CurrentLevelSelectController != null)
                {
                    Debug.Log("LobbyManager.instance.lobbySlots " + LobbyManager.instance.lobbySlots.Length);
                    LobbyManager.instance.maxPlayers = PlayerManager.maxPlayers;
                    LobbyManager.instance.maxPlayersPerConnection = PlayerManager.maxPlayers;

                    if (LobbyManager.instance.lobbySlots.Length < PlayerManager.maxPlayers)
                    {
                        Array.Resize(ref LobbyManager.instance.lobbySlots, PlayerManager.maxPlayers);
                    }
                }

                if (StatTracker.Instance != null)
                {
                    if (StatTracker.Instance.saveStatuses.Length < PlayerManager.maxPlayers)
                    {
                        Array.Resize(ref StatTracker.Instance.saveStatuses, PlayerManager.maxPlayers);
                    }
                    if (StatTracker.Instance.saveFiles.Length < PlayerManager.maxPlayers)
                    {
                        Array.Resize(ref StatTracker.Instance.saveFiles, PlayerManager.maxPlayers);
                    }
                }
            }
            catch (Exception xx)
            {
                Debug.Log("fixup error " + xx);
            }
        }

        static public void patchAll()
        {
            ControllerDisconnect_Patch.patch();
            Controller_Patch.patch();
            GraphScoreBoard_Patch.patch();
            InputManager_Patch.patch();
            InventoryBook_Patch.patch();
            KickTracker_Patch.patch();
            LevelPortal_Patch.patch();
            LevelSelectController_Patch.patch();
            LobbyPointCounter_Patch.patch();
            LobbySkillTracker_Patch.patch();
            PartyBox_Patch.patch();
            StatTracker_Patch.patch();
            VersusControl_Patch.patch();
        }
    }
}
