﻿using static TOHFE.Options;

namespace TOHFE.Roles.AddOns.Impostor;
public static class Mimic
{
    private const int Id = 23100;

    private static OptionItem CanSeeDeadRolesOpt;
    public static void SetupCustomOption()
    {
        SetupAdtRoleOptions(Id, CustomRoles.Mimic, canSetNum: true, tab: TabGroup.Addons);
        CanSeeDeadRolesOpt = BooleanOptionItem.Create(Id + 10, "MimicCanSeeDeadRoles", true, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Mimic]);
    }

    public static bool CanSeeDeadRoles(PlayerControl seer, PlayerControl target) => seer.Is(CustomRoles.Mimic) && CanSeeDeadRolesOpt.GetBool() && Main.VisibleTasksCount && !target.IsAlive();
}