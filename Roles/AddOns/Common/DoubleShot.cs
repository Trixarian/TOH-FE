using static TOHFE.Options;

namespace TOHFE.Roles.AddOns.Common;

public static class DoubleShot
{
    public static HashSet<byte> IsActive = [];

    public static OptionItem ImpCanBeDoubleShot;
    public static OptionItem CrewCanBeDoubleShot;
    public static OptionItem NeutralCanBeDoubleShot;

    public static void SetupCustomOption()
    {
        SetupAdtRoleOptions(19200, CustomRoles.DoubleShot, canSetNum: true, tab: TabGroup.ModifierSettings);
        ImpCanBeDoubleShot = BooleanOptionItem.Create(19203, "ImpCanBeDoubleShot", true, TabGroup.ModifierSettings, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.DoubleShot]);
        CrewCanBeDoubleShot = BooleanOptionItem.Create(19204, "CrewCanBeDoubleShot", true, TabGroup.ModifierSettings, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.DoubleShot]);
        NeutralCanBeDoubleShot = BooleanOptionItem.Create(19205, "NeutralCanBeDoubleShot", true, TabGroup.ModifierSettings, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.DoubleShot]);
    }
    public static void Init()
    {
        IsActive = [];
    }
}