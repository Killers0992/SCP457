using Exiled.API.Features;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCP457.Patches
{
	[HarmonyPatch(typeof(Scp049_2PlayerScript), nameof(Scp049_2PlayerScript.CallCmdShootAnim))]
	internal static class SCP0492AttackPatch
	{
		private static bool Prefix(Scp049_2PlayerScript __instance)
		{
			if (__instance._hub.gameObject.GetComponent<SCP457Controller>() != null)
			{
				Transform playerCameraReference = __instance._hub.PlayerCameraReference;
				Collider[] array = Physics.OverlapSphere(playerCameraReference.position + playerCameraReference.forward * 1.25f, MainClass.singleton.Config.attack_settings.radius_attack, LayerMask.GetMask(new string[]
				{
					"PlyCenter"
				}));
				HashSet<GameObject> hashSet = new HashSet<GameObject>();
				foreach (Collider collider in array)
				{
					global::ReferenceHub componentInParent3 = collider.GetComponentInParent<global::ReferenceHub>();
					if (!(componentInParent3 == null) && !(componentInParent3 == __instance._hub) && !componentInParent3.characterClassManager.IsAnyScp() && hashSet.Add(componentInParent3.gameObject) && !Physics.Linecast(__instance._hub.transform.position, componentInParent3.transform.position, LayerMask.GetMask(new string[]
						{
							"Default"
						})))
					{
						var burningComponent = componentInParent3.gameObject.GetComponent<BurningComponent>();
						if (burningComponent != null)
						{
							if (!Physics.Linecast(__instance._hub.playerMovementSync.RealModelPosition, componentInParent3.gameObject.transform.position, __instance._hub.playerMovementSync.CollidableSurfaces))
							{
								var burn = burningComponent.burningtime;
								burn += MainClass.singleton.Config.attack_settings.burning_time;
								burningComponent.burningAppliedBy = __instance._hub;
								if (burn > MainClass.singleton.Config.attack_settings.burning_time_max)
									burn = MainClass.singleton.Config.attack_settings.burning_time_max;
								burningComponent.burningtime = burn;
								burningComponent.colatime = MainClass.singleton.Config.attack_settings.cola_duration;
								burningComponent.hub.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Scp207>(MainClass.singleton.Config.attack_settings.cola_duration);
								burningComponent.hub.ReferenceHub.playerEffectsController.GetEffect<CustomPlayerEffects.Scp207>().ServerChangeIntensity(1);
								burningComponent.hub.ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(MainClass.singleton.Config.attack_settings.dmg_amount, "SCP457", DamageTypes.Asphyxiation, 0), burningComponent.hub.GameObject);
							}
						}
					}
				}
				return false;
			}
			return true;
		}
	}
}
