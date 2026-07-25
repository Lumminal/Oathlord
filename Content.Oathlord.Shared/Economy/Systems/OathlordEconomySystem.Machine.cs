using Content.Oathlord.Shared.Economy.Components;

namespace Content.Oathlord.Shared.Economy.Systems;

/// <summary>
/// Handles BUI events for the economy machine (depositing, withdrawing etc).
/// </summary>
public sealed partial class OathlordEconomySystem
{
    private void InitializeMachine()
    {
        Subs.BuiEvents<EconomyMachineComponent>(EconomyMachineUiKey.Key,
            subs =>
            {
                subs.Event<EconomyDepositMessage>(OnDeposit);
                subs.Event<EconomyWithdrawMessage>(OnWithdraw);
            });
    }

    private void OnDeposit(Entity<EconomyMachineComponent> ent, ref EconomyDepositMessage args)
    {
        if (args.DepositEntity is not { } depositEntity)
            return;

        var user = GetEntity(depositEntity);
        if (!_econAccountQuery.TryComp(user, out var account))
            return;

        // TODO: Check that we are within the correct economy...?
        // TODO: Spawn physical currency outside the machine
        if (GetCurrentEconomy(user) is not { } currentEconomy)
            return;

        if (GetTotalEconomyStored(currentEconomy) < args.Amount) // We tried to input an amount higher than the economy's budget...
            return;

        AddCurrencyToAccount((user, account), args.Amount);
    }

    private void OnWithdraw(Entity<EconomyMachineComponent> ent, ref EconomyWithdrawMessage args)
    {
        if (args.WithdrawEntity is not { } withdrawEntity)
            return;

        var entity = GetEntity(withdrawEntity);
        if (!_econAccountQuery.TryComp(entity, out var account))
            return;

        WithdrawFromAccount((entity, account), args.Amount);
    }
}
