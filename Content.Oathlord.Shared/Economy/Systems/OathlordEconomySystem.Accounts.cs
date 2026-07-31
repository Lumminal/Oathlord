using Content.Oathlord.Shared.Economy.Components;
using Content.Oathlord.Shared.Economy.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Economy.Systems;

/// <summary>
/// Public API for anything related to entities with <see cref="EconomyAccountComponent"/>
///
/// Accounts are part of the economy and rely on the bank for money.
///
/// Withdrawing will take money out of the bank (and therefore, reduce the amount stored on the user's account)
/// to give the player a physical version of the money.
///
/// Depositing will take money out of the bank and store it in the player's account.
///
/// Accounts are their own little "bank", while economy stores the central budget. If the economy needs money, they can
/// fine people to take back from the accounts, for example. So in general,
///
/// Accounts don't store individual coins like the economy's bank, rather they store one value.
/// Just like euros, as an example. Your account has 5000euros, not 10 * 50 + 9 * 500 euro bills.
/// But when withdrawing, you can withdraw however many bill types you want. As well when depositing, you can deposit only what you have on you.
/// The economy holds the physical version of the currencies in their bank (vault/treasury).
/// </summary>
public sealed partial class OathlordEconomySystem
{
    #region Public API

    /// <summary>
    /// Adds a specified amount of currency to an account
    /// TODO: if negative, and stored is 0 then it should get added to debt variable
    /// </summary>
    /// <param name="ent">The account to withdraw from</param>
    /// <param name="amount">The amount to adjust</param>
    public void AddCurrencyToAccount(Entity<EconomyAccountComponent?> ent, int amount)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp) || amount <= 0)
            return;

        ent.Comp.Stored += amount;
        Dirty(ent);
    }

    /// <summary>
    /// Withdraws a specified amount of currency to an account.
    /// </summary>
    /// <param name="ent">The account to withdraw from</param>
    /// <param name="amount">The amount to withdraw, make sure its positive as negative values or zero won't work</param>
    public void WithdrawFromAccount(Entity<EconomyAccountComponent?> ent, int amount)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp) || amount <= 0)
            return;

        ent.Comp.Stored = Math.Clamp(ent.Comp.Stored - amount, 0, int.MaxValue); // TODO: There should be a cap instead of int.MaxValue
        Dirty(ent);
    }

    /// <summary>
    /// Withdraws a specified amount of currencies from an account.
    /// </summary>
    /// <param name="ent">The account to withdraw from</param>
    /// <param name="toGive">The amount of currencies to withdraw from the economy, to give to the player</param>
    /// <returns>If the withdrawing was successful</returns>
    public bool WithdrawFromAccount(Entity<EconomyAccountComponent?> ent, Dictionary<ProtoId<EconomyCurrencyPrototype>, int> toGive)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp))
            return false;

        // We check that we can withdraw that much from our account
        // If toGive is too much then it doesn't make sense to withdraw...
        var total= GetTotalFromCurrencies(ent.Comp.Stored, toGive);
        if (total == 0)
            return false;

        // Accounts don't store individual currencies, only economy does.
        // Unlike depositing, withdrawing automatically also withdraws from the vault and gives you the physical coins directly (subject to change).
        if (GetCurrentEconomy(ent.Owner) is not { } economy || !TryWithdrawFromEconomy(economy.AsNullable(), toGive))
            return false;

        ent.Comp.Stored = Math.Clamp(ent.Comp.Stored - total, 0, int.MaxValue);
        Dirty(ent);

        Log.Info($"Withdrew: {toGive}. New amount is {ent.Comp.Stored}");
        return true;
    }

    /// <summary>
    /// Deposits a specified amount of currencies to an account.
    /// </summary>
    /// <param name="ent">The account to deposit to</param>
    /// <param name="toDeposit">The amount of currencies to withdraw from the economy, to deposit to the account</param>
    public void DepositToAccount(Entity<EconomyAccountComponent?> ent, Dictionary<ProtoId<EconomyCurrencyPrototype>, int> toDeposit)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        // Here, unlike withdrawing, we don't have to check for how much we have stored, since it doesn't matter (we're adding up)
        var total= GetTotalFromCurrencies(toDeposit);
        if (total == 0)
            return;

        // Do note that it is the banker's (or steward's) role to put the deposited amount into the vault.
        // The interaction should go like this:
        // Player gives X Na -> Banker puts Na into Vault -> Vault increases in value -> Banker increases account's value by X
        if (GetCurrentEconomy(ent.Owner) == null)
            return;

        ent.Comp.Stored = Math.Clamp(ent.Comp.Stored + total, 0, int.MaxValue);
        Dirty(ent);

        Log.Info($"Deposited: {toDeposit}. New amount is {ent.Comp.Stored}");
    }

    #endregion
}
