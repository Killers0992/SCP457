using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCP457
{
    public class PConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public SCP457Settings scp457_settings { get; set; } = new SCP457Settings();
        public SCP457Burning burning_settings { get; set; } = new SCP457Burning();
        public SCP457Attack attack_settings { get; set; } = new SCP457Attack();
        public CommandsData commands { get; set; } = new CommandsData();
    }

    public class SCP457Settings
    {
        public float health { get; set; } = 1100;
        public int chance_of_spawn { get; set; } = 25;
        public string spawn_location { get; set; } = "HCZ_ARMORY";
        public float size { get; set; } = 1.15f;
        public float burning_status_radius { get; set; } = 11.5f;
        public float scp457_info_duration { get; set; } = 15f;
        public string scp457_info { get; set; } = "\n\n\n<color=red>SCP 457</color> kill everyone.";
        public BadgeData badge { get; set; } = new BadgeData();
    }

    public class BadgeData
    {
        public string text { get; set; } = "SCP-457";
        public string color { get; set; } = "tomato";
    }

    public class SCP457Burning
    {
        public float dmg_delay { get; set; } = 1f;
        public float dmg_amount { get; set; } = 5f;
    }

    public class SCP457Attack
    {
        public float radius_attack { get; set; } = 3.5f;
        public float dmg_amount { get; set; } = 10f;
        public float cola_duration { get; set; } = 3f;
        public float burning_time { get; set; } = 5f;
        public float burning_time_max { get; set; } = 30f;
    }

    public class CommandsData
    {
        public CombustCommand combust { get; set; } = new CombustCommand();
    }

    public class CombustCommand
    {
        public float cooldown { get; set; } = 30f;
        public float dmg_amount { get; set; } = 15f;
        public float cola_duration { get; set; } = 3f;
        public float burning_time { get; set; } = 12f;
        public float burning_time_max { get; set; } = 30f;
        public string command_used_message { get; set; } = "<color=green>Done.</color>";
        public string cooldown_message { get; set; } = "<color=green>Wait %seconds% to use that command again.</color>";
    }
}
