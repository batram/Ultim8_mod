using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace Ultim8_mod
{
    class InventoryBook_Patch : InventoryBook
	{
		public static void patch()
        {
			MethodInfo a = typeof(InventoryBook).GetMethod("HasCursor", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo b = typeof(InventoryBook_Patch).GetMethod("HasCursor", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a	+ " b " + b);
			new Detour(a, b);

			a = typeof(InventoryBook).GetMethod("GetCursor", BindingFlags.Instance | BindingFlags.Public);
			b = typeof(InventoryBook_Patch).GetMethod("GetCursor", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);

			a = typeof(InventoryBook).GetMethod("AddPlayer", BindingFlags.Instance | BindingFlags.Public);
			b = typeof(InventoryBook_Patch).GetMethod("AddPlayer", BindingFlags.Instance | BindingFlags.Public);
			Debug.Log("a " + a + " b " + b);
			new Detour(a, b);
		}

		/* function InventoryBook.HasCursor hardcoded 4 comparison */
		new public bool HasCursor(int PlayerNumber)
		{
			if (PlayerNumber < 1 || PlayerNumber > PlayerManager.maxPlayers)
			{
				return false;
			}
			foreach (PickCursor pickCursor in this.cursors)
			{
				if (pickCursor != null && PlayerNumber == pickCursor.networkNumber)
				{
					return true;
				}
			}
			return false;
		}

		/* function InventoryBook.GetCursor hardcoded 4 comparison */
		new public PickCursor GetCursor(int PlayerNumber)
		{	
			if (PlayerNumber < 1 || PlayerNumber > PlayerManager.maxPlayers)
			{
				return null;
			}
			foreach (PickCursor pickCursor in this.cursors)
			{
				if (pickCursor != null && PlayerNumber == pickCursor.networkNumber)
				{
					return pickCursor;
				}
			}
			return null;
		}

		/* function InventoryBook.AddPlayer hardcoded 4 comparison, fixup cursor spawn location for additional players */
		new public PickCursor AddPlayer(int localPlayerNumber, int networkPlayerNumber, Controller input, Character.Animals animal)
		{
			if (localPlayerNumber > PlayerManager.maxPlayers || localPlayerNumber < 1)
			{
				return null;
			}
			PickCursor pickCursor = null;
			foreach (PickCursor pickCursor2 in this.cursors)
			{
				if (pickCursor2 != null && pickCursor2.networkNumber == networkPlayerNumber)
				{
					pickCursor = pickCursor2;
				}
			}
			if (pickCursor == null)
			{
				pickCursor = UnityEngine.Object.Instantiate<PickCursor>(this.pickCursorPrefab);
			}
			Debug.Log("PickCursor AddPlayer");
			AkSoundEngine.SetSwitch("Character", animal.ToString(), base.gameObject);
			pickCursor.transform.parent = base.transform;
			/* fixup cursor spawn location for additional players */
			pickCursor.transform.localPosition = this.cursorSpawnLocation[(localPlayerNumber > 4) ? 0 : (localPlayerNumber - 1)].localPosition;
			pickCursor.InventoryBookMenu = this;
			pickCursor.SetBounds(new Bounds(this.currentResolutionBoundingBox.center / base.transform.localScale.x, this.currentResolutionBoundingBox.size / base.transform.localScale.x));
			pickCursor.NetworknetworkNumber = networkPlayerNumber;
			pickCursor.NetworklocalNumber = localPlayerNumber;
			pickCursor.LocalPlayer = PlayerManager.GetInstance().GetPlayer(pickCursor.localNumber);
			pickCursor.SetSprites(animal);
			pickCursor.UseCamera = this.UiCamera.GetComponent<Camera>();
			pickCursor.Disable(true, false);
			Transform[] componentsInChildren = pickCursor.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = 5;
			}
			Debug.Log("PickCursor AddPlayer pickCursor");
			pickCursor.SetLayer(5, true);
			input.AddReceiver(pickCursor);
			this.cursors.Add(pickCursor);
			return pickCursor;
		}
	}
}
