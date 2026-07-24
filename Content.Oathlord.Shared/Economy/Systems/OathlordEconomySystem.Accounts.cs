using Content.Oathlord.Shared.Economy.Components;

namespace Content.Oathlord.Shared.Economy.Systems;

/// <summary>
/// Public API for anything related to entities with <see cref="EconomyAccountComponent"/>
/// </summary>
public sealed partial class OathlordEconomySystem
{
    /// <summary>
    /// Adds a specified amount of currency (Nar) to the account
    /// TODO: if negative, and stored is 0 then it should get added to debt variable
    /// </summary>
    /// <param name="ent">The account to withdraw from</param>
    /// <param name="amount">The amount to adjust</param>
    public void AdjustCurrencyFromAccount(Entity<EconomyAccountComponent?> ent, int amount)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        ent.Comp.Stored += amount;
        Dirty(ent);
    }

    /// <summary>
    /// Withdraws a specified amount of currency from the account.
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
}
