using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class Controller_Patch
	{
		public static void patch()
        {
			MethodInfo a = typeof(Controller).GetMethod("AddPlayer", BindingFlags.Instance | BindingFlags.Public);
			var b = typeof(Controller_Patch).GetMethod("AddPlayer", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			a = typeof(Controller).GetMethod("AssociateCharacter", BindingFlags.Instance | BindingFlags.Public);
			b = typeof(Controller_Patch).GetMethod("AssociateCharacter", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			a = typeof(Controller).GetMethod("ClearPlayers", BindingFlags.Instance | BindingFlags.Public);
			b = typeof(Controller_Patch).GetMethod("ClearPlayers", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			a = typeof(Controller).GetMethod("RemovePlayer", BindingFlags.Instance | BindingFlags.Public);
			b = typeof(Controller_Patch).GetMethod("RemovePlayer", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			a = typeof(Controller).GetMethod("GetAssociatedCharacters", BindingFlags.Instance | BindingFlags.Public);
			b = typeof(Controller_Patch).GetMethod("GetAssociatedCharacters", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			a = typeof(Controller).GetMethod("GetLastPlayerNumber", BindingFlags.Instance | BindingFlags.Public);
			b = typeof(Controller_Patch).GetMethod("GetLastPlayerNumber", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			a = typeof(Controller).GetMethod("GetLastPlayerNumberAfter", BindingFlags.Instance | BindingFlags.Public);
			b = typeof(Controller_Patch).GetMethod("GetLastPlayerNumberAfter", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);
		}

		public void AddPlayer(int player)
		{
			var prop = this.GetType().GetField("Player", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var Player = (int) prop.GetValue(this);

			if (player < 1 || player > PlayerManager.maxPlayers)
			{
				return;
			}
			Player |= 1 << player - 1;
			prop.SetValue(this, Player);
		}

		public int GetLastPlayerNumber()
		{
			var prop = this.GetType().GetField("Player", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var Player = (int)prop.GetValue(this);

			for (int i = PlayerManager.maxPlayers - 1; i >= 0; i--)
			{
				if ((Player & 1 << i) > 0)
				{
					return i + 1;
				}
			}
			return 0;
		}

		public int GetLastPlayerNumberAfter(int lastPlayerNumber)
		{
			var prop = this.GetType().GetField("Player", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var Player = (int)prop.GetValue(this);

			bool flag = false;
			for (int i = PlayerManager.maxPlayers - 1; i >= 0; i--)
			{
				if ((Player & 1 << i) > 0 && (flag || lastPlayerNumber == i + 1))
				{
					if (flag)
					{
						return i + 1;
					}
					flag = true;
				}
			}
			return 0;
		}

		public void AssociateCharacter(Character.Animals character, int player)
		{
			var prop = this.GetType().GetField("Player", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var Player = (int)prop.GetValue(this);
			
			var prop2 = this.GetType().GetField("associatedChars", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var associatedChars = prop2.GetValue(this) as Character.Animals[]; 

			if (player < 1 || player > PlayerManager.maxPlayers || (Player & 1 << player - 1) == 0)
			{
				return;
			}
			associatedChars[player - 1] = character;
		}

		public void ClearPlayers()
		{
			var prop = this.GetType().GetField("Player", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			prop.SetValue(this, 0);

			var prop2 = this.GetType().GetField("associatedChars", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			prop2.SetValue(this, new Character.Animals[PlayerManager.maxPlayers]);
		}

		public void RemovePlayer(int player)
		{
			var Player_field = this.GetType().GetField("Player", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			Player_field.SetValue(this, 0);
			var PossibleNetWorkNumber_field = this.GetType().GetField("PossibleNetWorkNumber", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			PossibleNetWorkNumber_field.SetValue(this, 0);

			var associatedChars_field = this.GetType().GetField("associatedChars", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var associatedChars = associatedChars_field.GetValue(this) as Character.Animals[];
			associatedChars[player - 1] = Character.Animals.NONE;
			associatedChars_field.SetValue(this, associatedChars);
		}

		public Character.Animals[] GetAssociatedCharacters()
		{
			var prop2 = this.GetType().GetField("associatedChars", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var associatedChars = prop2.GetValue(this) as Character.Animals[];

			if (associatedChars.Length < PlayerManager.maxPlayers)
            {
				Array.Resize(ref associatedChars, PlayerManager.maxPlayers);
				prop2.SetValue(this, associatedChars);
			}

			return associatedChars;
		}

	}
}
