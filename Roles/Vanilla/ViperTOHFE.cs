using AmongUs.GameOptions;

namespace TOHFE.Roles.Vanilla;

internal class ViperTOHFE : RoleBase
{
    //===========================SETUP================================\\
    public override CustomRoles Role => CustomRoles.ViperTOHFE;
    private const int Id = 35000;
    public override CustomRoles ThisRoleBase => CustomRoles.Viper;
    public override Custom_RoleType ThisRoleType => Custom_RoleType.ImpostorVanilla;
    //==================================================================\\

    private static OptionItem ViperDissolveTime;

    public override void SetupCustomOption()
    {
        Options.SetupRoleOptions(Id, TabGroup.ImpostorRoles, CustomRoles.ViperTOHFE);
        ViperDissolveTime = IntegerOptionItem.Create(Id + 2, GeneralOption.ViperBase_ViperDissolveTime, new(1, 180, 1), 15, TabGroup.ImpostorRoles, false)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.ViperTOHFE])
            .SetValueFormat(OptionFormat.Seconds);
    }

    public override void ApplyGameOptions(IGameOptions opt, byte playerId)
    {
        AURoleOptions.ViperDissolveTime = ViperDissolveTime.GetInt();
    }
}
