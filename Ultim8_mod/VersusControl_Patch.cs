using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    public class VersusControl_Patch
	{
		public static void patch()
		{

			//VersusControl.PreStartClient
			MethodInfo VersusControl = typeof(VersusControl).GetMethod("PreStartClient", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			MethodInfo VersusControlr = typeof(VersusControl_Patch).GetMethod("PreStartClient", BindingFlags.Instance | BindingFlags.NonPublic);
			new Detour(VersusControl, VersusControlr);

			//VersusControl.playersLeftToPlace
			PropertyInfo ca = typeof(VersusControl).GetProperty("playersLeftToPlace", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			MethodInfo[] afsw = ca.GetAccessors(true);
			var VersusControlplayersLeftToPlace = afsw[0];
			MethodInfo VersusControlplayersLeftToPlacer = typeof(VersusControl_Patch).GetMethod("playersLeftToPlace", BindingFlags.Instance | BindingFlags.NonPublic);
			new Detour(VersusControlplayersLeftToPlace, VersusControlplayersLeftToPlacer);
		}

		/* function VersusControl.playersLeftToPlace hardcoded 4 comparison*/
		protected bool playersLeftToPlace()
		{
			for (int num = 0; num != PlayerManager.maxPlayers; num++)
			{
				var prop2 = this.GetType().GetField("RemainingPlacements", BindingFlags.NonPublic | BindingFlags.Instance);
				var RemainingPlacements = prop2.GetValue(this) as int[];

				if (RemainingPlacements[num] > 0)
				{
					return true;
				}
			}
			return false;

		}

		/* class VersusControl hardcoded 4
			winOrder = new GamePlayer[4];
			RemainingPlacements = new int[4];
		 */
		protected virtual void PreStartClient()
		{
			Debug.Log(" VersusControl ");
			var prop = this.GetType().GetField("winOrder", BindingFlags.NonPublic | BindingFlags.Instance);
			prop.SetValue(this, new GamePlayer[PlayerManager.maxPlayers]);
			var prop2 = this.GetType().GetField("RemainingPlacements", BindingFlags.NonPublic | BindingFlags.Instance);
			prop2.SetValue(this, new int[PlayerManager.maxPlayers]);
		}
	}
}
