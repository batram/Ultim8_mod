using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class ControllerDisconnect_Patch : ControllerDisconnect
    {
        public static void patch()
        {
            MethodInfo a = typeof(ControllerDisconnect).GetMethod("Start", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo b = typeof(ControllerDisconnect_Patch).GetMethod("Start", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Debug.Log("a " + a + " b " + b);
            new Detour(a, b);

            a = typeof(ControllerDisconnect).GetMethod("SetPromptForPlayer", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            b = typeof(ControllerDisconnect_Patch).GetMethod("SetPromptForPlayer", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Debug.Log("a " + a + " b " + b);
            new Detour(a, b);

            Debug.Log("ControllerDisconnect patch");
        }

        new public static void SetPromptForPlayer(int playerNumber, bool shown)
        {
            Debug.Log("SetPromptForPlayer " + playerNumber);
            try
            {
                if (playerNumber >= 1 && playerNumber <= PlayerManager.maxPlayers)
                {
                    var prop2 = typeof(ControllerDisconnect).GetField("showingPrompts", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                    var showingPrompts = prop2.GetValue(null) as bool[];
                    showingPrompts[playerNumber - 1] = shown;
                }
            }
            catch ( Exception e)
            {
                Debug.Log("SetPromptForPlayer e " + playerNumber + " :: " + e);
            }
        }

        private void Start()
        {
            try
            {
                var prop3 = typeof(ControllerDisconnect).GetField("showingPrompts", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                var showingPrompts = prop3.GetValue(null) as bool[];

                if (showingPrompts.Length != PlayerManager.maxPlayers)
                {
                    Array.Resize(ref showingPrompts, PlayerManager.maxPlayers);
                    prop3.SetValue(null, showingPrompts);
                    var prop4 = typeof(ControllerDisconnect).GetField("orphanedReceivers", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    var orphanedReceivers = prop4.GetValue(this) as List<InputReceiver>[];
                    if(orphanedReceivers.Length != PlayerManager.maxPlayers)
                    {
                        var og_len = orphanedReceivers.Length;
                        Array.Resize(ref orphanedReceivers, PlayerManager.maxPlayers);
                        for (var i = og_len; i < orphanedReceivers.Length; i++)
                        {
                            orphanedReceivers[i] = new List<InputReceiver>();
                        }
                    }
                    prop4.SetValue(this, orphanedReceivers);

                    var prop5 = typeof(ControllerDisconnect).GetField("orphanedCharacters", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    prop5.SetValue(this, new Character.Animals[PlayerManager.maxPlayers][]);
                }

                if (this.ConnectPrompts.Length != PlayerManager.maxPlayers)
                {
                    var oglen = this.ConnectPrompts.Length;
                    Array.Resize<XboxReconnectPrompt>(ref ConnectPrompts, PlayerManager.maxPlayers);

                    for (var i = oglen; i < PlayerManager.maxPlayers; i++)
                    {
                        this.ConnectPrompts[i] = this.ConnectPrompts[0];
                    }
                }


                Controller.AddGlobalReceiver(this);
                GameEvent.GameEventManager.ChangeListener<GameEvent.ControllerConnectionEvent>(this, true);
                foreach (object obj in PlayerManager.GetInstance())
                {
                    Player player = (Player)obj;
                    if (player != null && player.UseController != null && !player.UseController.Connected)
                    {
                        showingPrompts[player.Number - 1] = true;
                    }
                }

                for (int num = 0; num != showingPrompts.Length; num++)
                {
                    if (showingPrompts[num])
                    {
                        this.ConnectPrompts[num].Show();
                    }
                    else
                    {
                        this.ConnectPrompts[num].Hide();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("CD exp " + e);
            }

        }
    }
}
