using System.Linq;
using Content.Oathlord.Shared.Economy.Components;
using Content.Oathlord.Shared.Economy.Prototypes;
using Content.Shared.Station;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Economy.Systems;

/// <summary>
/// This system-clutter handles all of the economic functions of Oathlord.
///
/// Oathlord has one main currency, Nar, which is used for the central economy.
/// In most cases, there can only be one economy (on the main map).
/// </summary>
public sealed partial class OathlordEconomySystem : EntitySystem
{
    [Dependency] private SharedStationSystem _station = default!;
    [Dependency] private IPrototypeManager _proto = default!;

    [Dependency] private EntityQuery<EconomyMapComponent> _econMapQuery = default!;
    [Dependency] private EntityQuery<EconomyAccountComponent> _econAccountQuery = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitializeMachine();
        InitializeCurrency();

        SubscribeLocalEvent<EconomyAccountComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<EconomyMapComponent, MapInitEvent>(OnEconomyMapInit);
    }

    private void OnMapInit(Entity<EconomyAccountComponent> ent, ref MapInitEvent args)
    {
        AddAccountToEconomy(ent.Owner);
    }

    private void OnEconomyMapInit(Entity<EconomyMapComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.TotalStored = GetTotalEconomyStored(ent.AsNullable());
        Dirty(ent);
    }

    /// <summary>
    /// Adds an account to all active economies.
    /// </summary>
    public void AddAccountToEconomy(EntityUid account)
    {
        // we could use current owning station instead,
        // but we don't know if someone spawns on a different map with no economy component,
        // so we have to use an entity query
        var econQuery = EntityQueryEnumerator<EconomyMapComponent>();
        while (econQuery.MoveNext(out var uid, out var mapEconomy))
        {
            mapEconomy.ActiveAccounts.Add(account);
            Dirty(uid, mapEconomy);
        }
    }

    /// <summary>
    /// Returns the economy the user is residing in.
    /// If the user is in another map without an economy, then this will return null.
    /// </summary>
    public Entity<EconomyMapComponent>? GetCurrentEconomy(EntityUid user)
    {
        if (_station.GetOwningStation(user) is not { } station || !_econMapQuery.TryComp(station, out var mapEconomy))
            return null;

        return (station, mapEconomy);
    }

    /// <summary>
    /// Returns the total amount of currency this economy has stored
    /// </summary>
    public int GetTotalEconomyStored(Entity<EconomyMapComponent?> ent)
    {
        if (!_econMapQuery.Resolve(ent.Owner, ref ent.Comp))
            return 0;

        var total = 0;
        foreach (var (currency, value) in ent.Comp.StoredCurrencies)
        {
            total += GetCurrencyTotal(currency, value);
        }

        ent.Comp.TotalStored = total;
        Dirty(ent);

        return total;
    }

    public bool TryWithdrawFromEconomy(Entity<EconomyMapComponent?> ent, int amount)
    {
        if (!_econMapQuery.Resolve(ent.Owner, ref ent.Comp))
            return false;

        var total = GetTotalEconomyStored(ent.AsNullable());
        if (total < amount)
            return false;

        WithdrawFromEconomy(ent.AsNullable(), amount);
        return true;
    }

    public void WithdrawFromEconomy(Entity<EconomyMapComponent?> ent, int amount)
    {
        if (!_econMapQuery.Resolve(ent.Owner, ref ent.Comp))
            return;
    }
}
