using Content.Oathlord.Shared.Economy.Components;
using Content.Oathlord.Shared.Economy.Prototypes;
using Robust.Shared.Prototypes;

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
    public void AddCurrencyToAccount(Entity<EconomyAccountComponent?> ent, int amount)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp) || amount <= 0)
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

    /// <summary>
    /// Withdraws a specified amount of currency from the account.
    /// </summary>
    /// <param name="ent">The account to withdraw from</param>
    /// <param name="toGive">The amount of currencies to withdraw from the economy, to give to the player</param>
    public IEnumerable<KeyValuePair<ProtoId<EconomyCurrencyPrototype>, int>>? WithdrawFromAccount(
        Entity<EconomyAccountComponent?> ent,
        Dictionary<ProtoId<EconomyCurrencyPrototype>, int> toGive)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp))
            return null;

        var total = 0;
        foreach (var (cur, amount) in toGive)
        {
            var curValue = GetCurrencyTotal(cur, amount);
            total += curValue;
        }

        if (total > ent.Comp.Stored)
        {
            Log.Info($"Tried to withdraw {total}, when we have {ent.Comp.Stored}");
            return null;
        }

        ent.Comp.Stored = Math.Clamp(ent.Comp.Stored - total, 0, int.MaxValue);
        Dirty(ent);

        return toGive;
    }
}
