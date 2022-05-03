using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace Ultim8_mod
{
    public class LevelPortal_Patch : LevelPortal
	{
		public static void patch()
		{
			//LevelPortal.Awake
			MethodInfo LevelPortalStart = typeof(LevelPortal).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo LevelPortalStartr = typeof(LevelPortal_Patch).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
			new Detour(LevelPortalStart, LevelPortalStartr);
		}

		/* class LevelPortal fudge with VoteArrows */
		new protected virtual void Awake()
		{
			VoteArrow[] componentsInChildren = base.GetComponentsInChildren<VoteArrow>();
			if (componentsInChildren.Length != GameSettings.GetInstance().MaxPlayers)
			{
				int num = componentsInChildren.Length;

				for (int j = num; j < GameSettings.GetInstance().MaxPlayers; j++)
				{
					Type type = componentsInChildren[3].GetType();
					VoteArrow voteArrow2 = componentsInChildren[3].gameObject.AddComponent(type) as VoteArrow;
					foreach (FieldInfo fieldInfo in type.GetFields())
					{
						fieldInfo.SetValue(voteArrow2, fieldInfo.GetValue(componentsInChildren[3]));
					}
				}
			}

		}
	}
}
