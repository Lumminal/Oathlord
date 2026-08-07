using Content.Oathlord.Shared.Economy.Components;
using Content.Shared.Popups;

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
                subs.Event<EconomyAddLoanMessage>(OnAddLoan);
                subs.Event<EconomyPayLoanMessage>(OnPayLoan);
            });
    }

    #region Event Handlers

    private void OnTransaction(Entity<EconomyMachineComponent> ent, ref EconomyTransactionMessage args)
    {
        // Using a switch statement here for 2 reasons:
        // 1. It's easy to read and modify for the future, 2. We know that the economy accepts a specific set of transactions (depositing and withdrawing).
        // So, modularizing it by making prototypes (or something similar) for now is overcomplicating things.
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

    private void OnAddLoan(Entity<EconomyMachineComponent> ent, ref EconomyAddLoanMessage args)
    {
        if (GetTransactEntity(args.TransactEntity) is not { } user)
            return;

        AddLoan(user, args.Loan);
    }

    private void OnPayLoan(Entity<EconomyMachineComponent> ent, ref EconomyPayLoanMessage args)
    {
        if (GetTransactEntity(args.TransactEntity) is not { } user|| args.Actor is not { Valid: true } actor)
            return;

        if (!TryPayLoan(user, args.Loan))
        {
            _popup.PopupCursor("Denied loan payment. Is there enough money in the account?", actor, PopupType.SmallCaution);
            // TODO: Play sound here
            return;
        }

        // TODO: Play sound here
        _popup.PopupCursor("The loan has been successfully paid!", actor, PopupType.Medium);
    }

    #endregion

    private void Withdraw(EntityUid machine, EconomyTransactionMessage args)
    {
        if (GetTransactEntity(args.TransactEntity) is not { } user || args.Actor is not { Valid: true } actor)
            return;

        var toTransact = args.ToTransact;
        if (!WithdrawFromAccount(user, toTransact, actor))
        {
            _popup.PopupCursor("Transaction denied. Invalid input, or not enough money in the bank.", actor, PopupType.SmallCaution);
            // TODO: Play sound here
            return;
        }

        // TODO: Play sound here
        SpawnPhysicalFromCurrencies(machine, toTransact);

        _popup.PopupCursor("Transaction successful. The currencies should appear on the ground!", actor, PopupType.Medium);
    }

    private void Deposit(EconomyTransactionMessage args)
    {
        if (GetTransactEntity(args.TransactEntity) is not { } user || args.Actor is not { Valid: true } actor)
            return;

        if (!DepositToAccount(user, args.ToTransact, actor))
        {
            _popup.PopupCursor("Transaction denied. Invalid input, or not enough money in the bank.", actor, PopupType.SmallCaution);
            // TODO: Play sound here
            return;
        }

        _popup.PopupCursor("Transaction successful. The account has been updated!", actor, PopupType.Medium);
        // TODO: Play sound here
    }

    /// <summary>
    /// Boilerplate to convert the transact entity from NetEntity to EntityUid
    /// </summary>
    private EntityUid? GetTransactEntity(NetEntity? entity)
    {
        if (entity is not { } withdrawEntity)
            return null;

        return GetEntity(withdrawEntity);
    }
}
