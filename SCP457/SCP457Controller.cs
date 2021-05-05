using CustomPlayerEffects;
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
    public class SCP457Controller : MonoBehaviour
    {
        public Player player;
        public List<CoroutineHandle> handlers = new List<CoroutineHandle>();

        public float combustdelay = 0f;

        public void Awake()
        {
            player = Player.Get(gameObject);
            Log.Debug($"Enabled scp457controller for player: {player.DisplayNickname} ({player.UserId}).");
            player.SetRole(RoleType.Scp0492);
            player.ShowHint(MainClass.singleton.Config.scp457_settings.scp457_info, MainClass.singleton.Config.scp457_settings.scp457_info_duration);
            if (player.GlobalBadge == null)
            {
                player.RankName = MainClass.singleton.Config.scp457_settings.badge.text;
                player.RankColor = MainClass.singleton.Config.scp457_settings.badge.color;
            }
            handlers.Add(Timing.RunCoroutine(UpdateBurn()));
            handlers.Add(Timing.RunCoroutine(UpdateDelay()));
        }
        public IEnumerator<float> UpdateDelay()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);

                try
                {
                    player.ClearInventory();
                    if (combustdelay != 0f)
                        combustdelay--;
                }
                catch (Exception ex) 
                { 
                    Log.Error(ex.ToString());
                }
            }
        }
        public IEnumerator<float> UpdateBurn()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(0.1f);
                try
                {
                    foreach (var plr in Player.List)
                    {
                        if (plr.GameObject.GetComponent<BurningComponent>() != null)
                        {
                            if (Vector3.Distance(plr.Position, player.Position) < MainClass.singleton.Config.scp457_settings.burning_status_radius)
                            {
                                if (!plr.ReferenceHub.characterClassManager.IsAnyScp())
                                {
                                    plr.GameObject.GetComponent<BurningComponent>().burningAppliedBy = player.ReferenceHub;
                                    plr.GameObject.GetComponent<BurningComponent>().burning = !Physics.Linecast(plr.Position, player.Position, plr.ReferenceHub.playerMovementSync.CollidableSurfaces);
                                }
                                else
                                    plr.GameObject.GetComponent<BurningComponent>().burning = false;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        public void Destroy()
        {
            foreach(var handler in handlers)
                Timing.KillCoroutines(handler);
            handlers.Clear();
            if (player != null)
            {
                player.Scale = new Vector3(1f, 1f, 1f);
                player.RankName = "";
            }
            Log.Debug("SCP457 controller disabled, scp killed or disconnected.");
        }
    }
}
