using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCP457.Patches
{
	[HarmonyPatch(typeof(Scp049_2PlayerScript), nameof(Scp049_2PlayerScript.CallCmdAttackWindow))]
	internal static class SCP0492AttackWindowPatch
	{
		private static bool Prefix(Scp049_2PlayerScript __instance, GameObject windowObj)
		{
			if (__instance._hub.gameObject.GetComponent<SCP457Controller>() != null)
				return false;
			return true;
		}
	}
}
