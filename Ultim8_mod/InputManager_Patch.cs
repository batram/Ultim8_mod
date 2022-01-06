using InControl;
using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class InputManager_Patch
	{
		public static void patch()
		{
			// InputManager.EnableNativeInput always true
			// This allows more than 4 controllers
			PropertyInfo ca = typeof(InputManager).GetProperty("EnableNativeInput", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			MethodInfo[] accessors = ca.GetAccessors(true);
			var get_EnableNativeInput = accessors[1];
			MethodInfo b = typeof(InputManager_Patch).GetMethod("getEnableNativeInput", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
			Debug.Log("a " + get_EnableNativeInput + " b " + b);
			new Detour(get_EnableNativeInput, b);
		}

		internal static bool getEnableNativeInput()
		{
			Debug.Log("PATCHED INputmanager getEnableNativeInput always true");
			return true;
		}
	}
}
