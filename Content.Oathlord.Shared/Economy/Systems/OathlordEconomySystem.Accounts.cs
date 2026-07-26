using Content.Oathlord.Shared.Economy.Components;
using Content.Oathlord.Shared.Economy.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Economy.Systems;

/// <summary>
/// Public API for anything related to entities with <see cref="EconomyAccountComponent"/>
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
    public void WithdrawFromAccount(Entity<EconomyAccountComponent?> ent, Dictionary<ProtoId<EconomyCurrencyPrototype>, int> toGive)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        var total= GetTotalFromCurrencies(ent.Comp.Stored, toGive);
        if (total == 0)
            return;

        if (GetCurrentEconomy(ent.Owner) is not { } economy)
            return;

        // Accounts don't store individual currencies, only economy does.
        // So we have to adjust the economy after a withdraw
        WithdrawFromEconomy(economy.AsNullable(), toGive);

        ent.Comp.Stored = Math.Clamp(ent.Comp.Stored - total, 0, int.MaxValue);
        Dirty(ent);
    }

    #endregion
}
