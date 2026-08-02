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
                subs.Event<EconomyAddLoanMessage>(OnAddLoan);
                subs.Event<EconomyPayLoanMessage>(OnPayLoan);
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

    private void OnAddLoan(Entity<EconomyMachineComponent> ent, ref EconomyAddLoanMessage args)
    {
        if (GetTransactEntity(args.TransactEntity) is not { } user)
            return;

        // validate loan in case of bad input...
        var loan = args.Loan;
        if (loan.Paid || loan.Amount <= 0 || loan.DueTime.TotalMinutes <= 0)
            return;

        AddLoan(user, args.Loan);
    }

    private void OnPayLoan(Entity<EconomyMachineComponent> ent, ref EconomyPayLoanMessage args)
    {
        if (GetTransactEntity(args.TransactEntity) is not { } user)
            return;

        // again, validate the loan
        var loan = args.Loan;
        if (loan.Paid || loan.Amount <= 0) // loan is already paid, or loan is negative (somehow)
            return;

        TryPayLoan(user, loan);
    }

    #endregion

    private void Withdraw(EntityUid machine, EconomyTransactionMessage args)
    {
        if (GetTransactEntity(args.TransactEntity) is not { } user)
            return;

        var toTransact = args.ToTransact;
        if (!WithdrawFromAccount(user, toTransact))
            return;

        // TODO: Play sound here
        SpawnPhysicalFromCurrencies(machine, toTransact);
    }

    private void Deposit(EconomyTransactionMessage args)
    {
        if (GetTransactEntity(args.TransactEntity) is not { } user)
            return;

        // TODO: Play sound here
        DepositToAccount(user, args.ToTransact);
    }

    private EntityUid? GetTransactEntity(NetEntity? entity)
    {
        if (entity is not { } withdrawEntity)
            return null;

        return GetEntity(withdrawEntity);
    }
}
