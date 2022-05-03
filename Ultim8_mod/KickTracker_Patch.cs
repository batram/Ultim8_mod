using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class KickTracker_Patch : KickTracker
	{
		public static void patch()
		{
			MethodInfo a = typeof(KickTracker).GetMethod("ClearPlayer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			MethodInfo b = typeof(KickTracker_Patch).GetMethod("ClearPlayer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			a = typeof(KickTracker).GetMethod("CountVotes", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			b = typeof(KickTracker_Patch).GetMethod("CountVotes", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			var ca = typeof(KickTracker).GetConstructors()[0];
			b = typeof(KickTracker_Patch).GetMethod("Inst", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(ca, b);	
		}

		/* constructor KickTracker hardcoded 4 comparison and size */
		public KickTracker Inst()
        {
			var prop = this.GetType().GetField("votes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

			var votes = new bool[PlayerManager.maxPlayers][];
			for (int num = 0; num != PlayerManager.maxPlayers; num++)
			{
				votes[num] = new bool[PlayerManager.maxPlayers];
			}
			prop.SetValue(this, votes);

			return this; 
		}

		/* function KickTracker.ClearPlayer hardcoded 4 comparison */
		new public void ClearPlayer(int player)
		{
			var prop = this.GetType().GetField("votes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var votes = prop.GetValue(this) as bool[][];
			for (int num = 0; num != PlayerManager.maxPlayers; num++)
			{
				votes[num][player - 1] = false;
				votes[player - 1][num] = false;
			}
		}

		/* function KickTracker.CountVotes hardcoded 4 comparison */
		new public int CountVotes(int targetPlayer)
		{
			var prop = this.GetType().GetField("votes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var votes = prop.GetValue(this) as bool[][];

			int num = 0;
			bool[] array = votes[targetPlayer - 1];
			for (int num2 = 0; num2 != PlayerManager.maxPlayers; num2++)
			{
				if (array[num2])
				{
					num++;
				}
			}
			return num;
		}

	}
}
