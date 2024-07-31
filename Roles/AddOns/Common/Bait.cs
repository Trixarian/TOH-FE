﻿using System;
using TOHFE.Modules;
using static TOHFE.Translator;
using static TOHFE.Options;

namespace TOHFE.Roles.AddOns.Common;

public static class Bait
{
    private const int Id = 18700;

    public static OptionItem ImpCanBeBait;
    public static OptionItem CrewCanBeBait;
    public static OptionItem NeutralCanBeBait;
    public static OptionItem BaitDelayMin;
    public static OptionItem BaitDelayMax;
    public static OptionItem BaitDelayNotify;
    public static OptionItem BaitNotification;
    public static OptionItem BaitCanBeReportedUnderAllConditions;
    
    public static List<byte> BaitAlive = [];

    public static void SetupCustomOptions()
    {
        SetupAdtRoleOptions(Id, CustomRoles.Bait, canSetNum: true);
        ImpCanBeBait = BooleanOptionItem.Create(Id + 10, "ImpCanBeBait", true, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Bait]);
        CrewCanBeBait = BooleanOptionItem.Create(Id + 11, "CrewCanBeBait", true, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Bait]);
        NeutralCanBeBait = BooleanOptionItem.Create(Id + 12, "NeutralCanBeBait", true, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Bait]);
        BaitDelayMin = FloatOptionItem.Create(Id + 13, "BaitDelayMin", new(0f, 5f, 1f), 0f, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Bait])
            .SetValueFormat(OptionFormat.Seconds);
        BaitDelayMax = FloatOptionItem.Create(Id + 14, "BaitDelayMax", new(0f, 10f, 1f), 0f, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Bait])
            .SetValueFormat(OptionFormat.Seconds);
        BaitDelayNotify = BooleanOptionItem.Create(Id + 15, "BaitDelayNotify", false, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Bait]);
        BaitNotification = BooleanOptionItem.Create(Id + 16, "BaitNotification", false, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Bait]);
        BaitCanBeReportedUnderAllConditions = BooleanOptionItem.Create(Id + 17, "BaitCanBeReportedUnderAllConditions", false, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Bait]);
    }

    public static void Init()
    {
        BaitAlive = [];
    }
    public static void BaitAfterDeathTasks(PlayerControl killer, PlayerControl target)
    {        
        if (killer.PlayerId == target.PlayerId)
        {
            if (target.GetRealKiller() != null)
            {
                if (!target.GetRealKiller().IsAlive()) return;
                killer = target.GetRealKiller();
            }
        }

        if (killer.PlayerId == target.PlayerId) return;

        if (killer.Is(CustomRoles.KillingMachine)
            || killer.Is(CustomRoles.Swooper)
            || killer.Is(CustomRoles.Wraith)
            || (killer.Is(CustomRoles.Oblivious) && Oblivious.ObliviousBaitImmune.GetBool()))
            return;

        {
            killer.RPCPlayCustomSound("Congrats");
            target.RPCPlayCustomSound("Congrats");
            float delay;
            if (BaitDelayMax.GetFloat() < BaitDelayMin.GetFloat()) delay = 0f;
            else delay = IRandom.Instance.Next((int)BaitDelayMin.GetFloat(), (int)BaitDelayMax.GetFloat() + 1);
            delay = Math.Max(delay, 0.15f);
            if (delay > 0.15f && BaitDelayNotify.GetBool()) killer.Notify(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Bait), string.Format(GetString("KillBaitNotify"), (int)delay)), delay);
            Logger.Info($"{killer.GetNameWithRole()} 击杀诱饵 => {target.GetNameWithRole()}", "MurderPlayer");
            _ = new LateTask(() => { if (GameStates.IsInTask && GameStates.IsInGame) killer?.CmdReportDeadBody(target.Data); }, delay, "Bait Self Report");
        }
    }
}

