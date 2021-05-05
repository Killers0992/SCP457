using Dissonance;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using GameCore;
using HarmonyLib;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCP457
{
    public class MainClass : Plugin<PConfig>
    {
        public override string Name { get; } = "SCP457";
        public override string Author { get; } = "Killers0992";
        public override string Prefix { get; } = "scp457";

        public static MainClass singleton;
        public bool is457chosen = false;

        public override void OnEnabled()
        {
            singleton = this;
            var harmony = new Harmony("com.scp457." + DateTime.Now.Ticks);
            harmony.PatchAll();
            Exiled.API.Features.Log.Info("Plugin SCP457 enabled.");
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += Server_SendingRemoteAdminCommand;
            Exiled.Events.Handlers.Server.SendingConsoleCommand += Server_SendingConsoleCommand;
            Exiled.Events.Handlers.Player.Joined += Player_Joined;
            Exiled.Events.Handlers.Player.Spawning += Player_Spawning;
            Exiled.Events.Handlers.Player.Left += Player_Left;
            Exiled.Events.Handlers.Player.Died += Player_Died;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Player.ChangingRole += Player_ChangingRole;
            Exiled.Events.Handlers.Player.Hurting += Player_Hurting;
            Exiled.Events.Handlers.Player.MedicalItemUsed += Player_MedicalItemUsed;
        }

        private void Player_Hurting(Exiled.Events.EventArgs.HurtingEventArgs ev)
        {
             if (ev.Target.GameObject.GetComponent<BurningComponent>() != null)
            {
                if (ev.HitInformations.GetDamageType() == DamageTypes.Scp207)
                {
                    if (ev.Target.ReferenceHub.playerEffectsController.GetEffect<CustomPlayerEffects.Scp207>().Enabled && ev.Target.GameObject.GetComponent<BurningComponent>().colatime != 0f)
                    {
                        ev.Amount = 0f;
                    }
                }
            }
        }

        private void Player_MedicalItemUsed(Exiled.Events.EventArgs.UsedMedicalItemEventArgs ev)
        {
            if (ev.Item == ItemType.Medkit)
                ev.Player.GameObject.GetComponent<BurningComponent>().burningtime = 0;
        }

        private void Server_SendingConsoleCommand(Exiled.Events.EventArgs.SendingConsoleCommandEventArgs ev)
        {
            if (ev.Name.ToUpper() == "COMBUST")
            {
                var controller = ev.Player.GameObject.GetComponent<SCP457Controller>();
                if (controller != null)
                {
                    if (controller.combustdelay == 0f)
                    {
                        foreach (var plr in Player.List)
                        {
                            var brn = plr.GameObject.GetComponent<BurningComponent>();
                            if (brn != null)
                            {
                                if (brn.burning)
                                {
                                    var temp = brn.burningtime;
                                    temp += Config.commands.combust.burning_time;
                                    if (temp < Config.commands.combust.burning_time_max)
                                    {
                                        plr.ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(MainClass.singleton.Config.commands.combust.dmg_amount, "SCP457", DamageTypes.Asphyxiation, 0), plr.GameObject);
                                        brn.burningtime = temp;
                                        brn.colatime = MainClass.singleton.Config.attack_settings.cola_duration;
                                        plr.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Scp207>(MainClass.singleton.Config.attack_settings.cola_duration);
                                        plr.ReferenceHub.playerEffectsController.GetEffect<CustomPlayerEffects.Scp207>().ServerChangeIntensity(1);
                                        plr.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Flashed>(1f);
                                    }
                                }
                            }
                        }
                        ev.Allow = false;
                        ev.ReturnMessage = Config.commands.combust.command_used_message;
                        controller.combustdelay = Config.commands.combust.cooldown;
                    }
                    else
                    {
                        ev.Allow = false;
                        ev.ReturnMessage = Config.commands.combust.cooldown_message.Replace("%seconds%", ((int)controller.combustdelay).ToString());
                    }
                }
            }
        }

        private void Server_SendingRemoteAdminCommand(Exiled.Events.EventArgs.SendingRemoteAdminCommandEventArgs ev)
        {
            if (ev.Name.ToUpper() == "SPAWN457")
            {
                ev.IsAllowed = false;
                if (ev.Sender.CheckPermission("scp457.spawn"))
                {
                    if (ev.Arguments.Count == 0)
                    {
                        ev.Sender.RemoteAdminMessage("Arguments: spawn457 <playerId>", true, "SCP457");
                    }
                    else
                    {
                        if (int.TryParse(ev.Arguments[0], out int id))
                        {
                            var plr = Player.Get(id);
                            if (plr != null)
                            {
                                if (plr.GameObject.GetComponent<SCP457Controller>() == null)
                                    plr.GameObject.AddComponent<SCP457Controller>();
                                ev.Sender.RemoteAdminMessage("Done.", true, "SCP457");
                                return;
                            }
                            ev.Sender.RemoteAdminMessage("Player not found.", true, "SCP457");
                        }
                        else
                        {
                            ev.Sender.RemoteAdminMessage("Arguments: spawn457 <playerId>", true, "SCP457");
                        }
                    }
                }
                else
                {
                    ev.Sender.RemoteAdminMessage("No Permission.", true, "SCP457");
                }
            }
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (RoundStart.RoundLenght.TotalSeconds < 15 && ev.NewRole != RoleType.Spectator && ev.Player.ReferenceHub.characterClassManager.IsAnyScp())
            {
                var gen = new System.Random(); 
                if (gen.Next(100) > Config.scp457_settings.chance_of_spawn && !is457chosen)
                {
                    is457chosen = true;
                    var controller = ev.Player.GameObject.GetComponent<SCP457Controller>();
                    if (controller == null)
                        ev.Player.GameObject.AddComponent<SCP457Controller>();
                    ev.NewRole = RoleType.Scp0492;
                    ev.Items.Clear();
                    return;
                }
            }
        }

        private void Server_WaitingForPlayers()
        {
            is457chosen = false;
        }

        private void Player_Died(Exiled.Events.EventArgs.DiedEventArgs ev)
        {
            var controller = ev.Target.GameObject.GetComponent<SCP457Controller>();
            ev.Target.GameObject.GetComponent<BurningComponent>().burningtime = 0;
            if (controller != null)
            {
                ev.Target.Scale = new Vector3(1f, 1f, 1f);
                controller.Destroy();
                GameObject x = null;
                foreach (GameObject gameObject in global::PlayerManager.players)
                {
                    if (gameObject.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId == ev.HitInformations.PlayerId)
                    {
                        x = gameObject;
                    }
                }
                var l = ev.Target.ReferenceHub.characterClassManager.Classes.ToList();
                int pd = 0;
                foreach(var p in l.ToArray())
                {
                    if (p.roleId == RoleType.Scp0492)
                    {
                        p.fullName = "SCP-457";
                        l[pd] = p;
                    }
                    pd++;
                }
                ev.Target.ReferenceHub.characterClassManager.Classes = l.ToArray();
                if (x != null)
                {
                    global::NineTailedFoxAnnouncer.AnnounceScpTermination(ev.Target.ReferenceHub.characterClassManager.CurRole, ev.HitInformations, string.Empty);
                }
                else
                {
                    global::DamageTypes.DamageType damageType = ev.HitInformations.GetDamageType();
                    if (damageType == global::DamageTypes.Tesla)
                    {
                        global::NineTailedFoxAnnouncer.AnnounceScpTermination(ev.Target.ReferenceHub.characterClassManager.CurRole, ev.HitInformations, "TESLA");
                    }
                    else if (damageType == global::DamageTypes.Nuke)
                    {
                        global::NineTailedFoxAnnouncer.AnnounceScpTermination(ev.Target.ReferenceHub.characterClassManager.CurRole, ev.HitInformations, "WARHEAD");
                    }
                    else if (damageType == global::DamageTypes.Decont)
                    {
                        global::NineTailedFoxAnnouncer.AnnounceScpTermination(ev.Target.ReferenceHub.characterClassManager.CurRole, ev.HitInformations, "DECONTAMINATION");
                    }
                    else
                    {
                        global::NineTailedFoxAnnouncer.AnnounceScpTermination(ev.Target.ReferenceHub.characterClassManager.CurRole, ev.HitInformations, "UNKNOWN");
                    }
                }
                UnityEngine.Object.Destroy(controller);
                foreach (var plr in Player.List)
                    if (plr.GameObject.GetComponent<BurningComponent>().burning)
                        plr.GameObject.GetComponent<BurningComponent>().burning = false;
            }
        }

        private void Player_Left(Exiled.Events.EventArgs.LeftEventArgs ev)
        {
            var controller = ev.Player.GameObject.GetComponent<SCP457Controller>();
            if (controller != null)
                controller.Destroy();
            var controller2 = ev.Player.GameObject.GetComponent<BurningComponent>();
            if (controller2 != null)
                controller2.Destroy();
        }

        private void Player_Spawning(Exiled.Events.EventArgs.SpawningEventArgs ev)
        {
            if (ev.RoleType == RoleType.Scp0492)
            {
                var controller = ev.Player.GameObject.GetComponent<SCP457Controller>();
                if (controller != null)
                {
                    global::Door door = UnityEngine.Object.FindObjectsOfType<global::Door>().FirstOrDefault((global::Door dr) => dr.DoorName.ToUpper() == Config.scp457_settings.spawn_location.ToUpper());
                    if (door == null)
                        return;
                    if (!global::PlayerMovementSync.FindSafePosition(door.transform.position, out Vector3 down, true))
                        return;
                    ev.Position = down;
                    Timing.CallDelayed(1.5f, () =>
                    {
                        ev.Player.Health = Config.scp457_settings.health;
                        ev.Player.MaxHealth = (int)Config.scp457_settings.health;
                        ev.Player.Scale = new Vector3(Config.scp457_settings.size, Config.scp457_settings.size, Config.scp457_settings.size);
                    });
                }
            } 
            else
            {
                var controller = ev.Player.GameObject.GetComponent<SCP457Controller>();
                if (controller != null)
                {
                    controller.Destroy();
                    UnityEngine.Object.Destroy(controller);
                }
            }
        }

        private void Player_Joined(Exiled.Events.EventArgs.JoinedEventArgs ev)
        {
            ev.Player.GameObject.AddComponent<BurningComponent>();
        }
    }
}
