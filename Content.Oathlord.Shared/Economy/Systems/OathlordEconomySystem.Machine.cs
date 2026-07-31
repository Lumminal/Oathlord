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
                subs.Event<EconomyTransactionMessage>(OnTransaction);
            });
    }

    #region Event Handlers

    private void OnTransaction(Entity<EconomyMachineComponent> ent, ref EconomyTransactionMessage args)
    {
        // Using a switch statement here for 2 reasons:
        // 1. It's easy to read and modify for the future
        // 2. We know that the economy accepts a specific set of transactions.
        // So, modularizing it by making prototypes (or something similar) is overcomplicating things.
        switch (args.Type)
        {
            case EconomyTransaction.Withdraw:
                Withdraw(ent.Owner, args);
                break;
            case EconomyTransaction.Deposit:
                Deposit(args);
                break;
        }
    }

    #endregion

    private void Withdraw(EntityUid machine, EconomyTransactionMessage args)
    {
        if (args.TransactEntity is not { } withdrawEntity)
            return;

        var toTransact = args.ToTransact;
        var user = GetEntity(withdrawEntity);

        if (!WithdrawFromAccount(user, toTransact))
            return;

        // TODO: Play sound here
        SpawnPhysicalFromCurrencies(machine, toTransact);
    }

    private void Deposit(EconomyTransactionMessage args)
    {
        if (args.TransactEntity is not { } depositEntity)
            return;

        // TODO: Play sound here
        var user = GetEntity(depositEntity);
        DepositToAccount(user, args.ToTransact);
    }
}
