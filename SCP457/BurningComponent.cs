using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCP457
{
    public class BurningComponent : MonoBehaviour
    {
        public Player hub;
        public List<CoroutineHandle> handlers = new List<CoroutineHandle>();

        public void Awake()
        {
            hub = Player.Get(this.gameObject);
            Log.Debug("Enabled burningcomponent for player: " + hub.Nickname + " (" + hub.UserId + ").");
            handlers.Add(Timing.RunCoroutine(UpdateBurningStatus()));
            handlers.Add(Timing.RunCoroutine(UpdateBurning()));
            handlers.Add(Timing.RunCoroutine(UpdateBurningStatus2()));
        }

        public IEnumerator<float> UpdateBurningStatus2()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                if (colatime != 0)
                {
                    colatime--;
                }
            }
        }


        public IEnumerator<float> UpdateBurningStatus()
        {
            while(true)
            {
                yield return Timing.WaitForSeconds(0.1f);
                if (burningAppliedBy != null)
                {
                    if (burningAppliedBy.gameObject.GetComponent<SCP457Controller>() != null)
                    {
                        if (!burning)
                        {
                            hub.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Burned>();
                        }
                        else
                        {
                            hub.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Burned>();
                            if (hub.ReferenceHub.characterClassManager.NetworkCurClass == RoleType.Spectator)
                                burning = false;
                        }
                    }
                    else
                        hub.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Burned>();
                }
                else
                    hub.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Burned>();
            }
        }

        public IEnumerator<float> UpdateBurning()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(MainClass.singleton.Config.burning_settings.dmg_delay);
                if (burningtime != 0)
                {
                    burningtime--;
                    if (burningAppliedBy != null)
                    {
                        if (burningAppliedBy.gameObject.GetComponent<SCP457Controller>() != null)
                        {
                            if (hub.Role != RoleType.Spectator)
                                burningAppliedBy.gameObject.GetComponent<Scp049_2PlayerScript>().TargetHitMarker(burningAppliedBy.characterClassManager.connectionToClient);
                        }
                    }
                    hub.ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(MainClass.singleton.Config.burning_settings.dmg_amount, "SCP457", DamageTypes.Asphyxiation, 0), hub.GameObject);
                }
            }
        }

        public ReferenceHub burningAppliedBy;
        public float burningtime = 0f;
        public float colatime = 0f;
        public bool burning = false;

        public void Destroy()
        {
            foreach(var handler in handlers)
                Timing.KillCoroutines(handler);
            handlers.Clear();
        }
    }
}
