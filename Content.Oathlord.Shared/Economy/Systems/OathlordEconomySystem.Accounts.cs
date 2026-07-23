using Content.Oathlord.Shared.Economy.Components;

namespace Content.Oathlord.Shared.Economy.Systems;

/// <summary>
/// Public API for anything related to entities with <see cref="EconomyAccountComponent"/>
/// </summary>
public sealed partial class OathlordEconomySystem
{
    /// <summary>
    /// Adds a specified amount of currency (Nar) to the account
    /// This can be also negative.
    /// TODO: if negative it gets added to debt
    /// </summary>
    public void AddCurrencyToAccount(Entity<EconomyAccountComponent?> ent, int amount)
    {
        if (!_econAccountQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        ent.Comp.Stored += amount;
        Dirty(ent);
    }
}
