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

    #region Event Handlers

    private void OnMapInit(Entity<EconomyAccountComponent> ent, ref MapInitEvent args)
    {
        AddAccountToEconomy(ent.Owner);
    }

    private void OnEconomyMapInit(Entity<EconomyMapComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.TotalStored = GetTotalEconomyStored(ent.AsNullable());
        Dirty(ent);
    }

    #endregion

    #region Public API

    /// <summary>
    /// Adds an account to all active economies.
    /// </summary>
    public void AddAccountToEconomy(EntityUid account)
    {
        // we could use current owning station instead,
        // but we don't know if someone spawns on a different map with no economy component,
        // so we have to use an entity query enumerator to get all economies
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

    /// <summary>
    /// Withdraws a specific amount of currencies from the economy.
    /// Does not accept negative values.
    /// </summary>
    public bool TryWithdrawFromEconomy(Entity<EconomyMapComponent?> ent, Dictionary<ProtoId<EconomyCurrencyPrototype>, int> toWithdraw)
    {
        if (!_econMapQuery.Resolve(ent.Owner, ref ent.Comp))
            return false;

        var total = GetTotalFromCurrencies(ent.Comp.TotalStored, toWithdraw);
        if (total == 0)
            return false;

        foreach (var (cur, storedValue) in ent.Comp.StoredCurrencies)
        {
            if (!toWithdraw.TryGetValue(cur, out var value) || value < 0 || value > storedValue)
            {
                return false;
            }

            ent.Comp.StoredCurrencies[cur] = Math.Clamp(ent.Comp.StoredCurrencies[cur] - toWithdraw[cur], 0, int.MaxValue);
        }

        Dirty(ent);
        return true;
    }

    /// <summary>
    /// Gets the loan interest specified by the economy
    /// </summary>
    public float GetLoanInterest(Entity<EconomyMapComponent?> ent)
    {
        if (!_econMapQuery.Resolve(ent.Owner, ref ent.Comp))
            return 0f;

        return ent.Comp.LoanInterest;
    }

    /// <summary>
    /// Gets the loan interest specified by the economy
    /// </summary>
    /// <param name="user">A user to get the economy from</param>
    public float GetLoanInterest(EntityUid user)
    {
        if (GetCurrentEconomy(user) is not { } economy)
            return 0f;

        return economy.Comp.LoanInterest;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Helper to get the total amount from currencies, and check if its enough versus our stored amount.
    /// Does not accept negative values.
    /// </summary>
    /// <param name="stored"></param>The total stored amount we have in our accounts or economy
    /// <param name="currencies"></param>The currencies to check against
    /// <returns></returns>
    private int GetTotalFromCurrencies(int stored, Dictionary<ProtoId<EconomyCurrencyPrototype>, int> currencies)
    {
        var total = 0;
        foreach (var (cur, amount) in currencies)
        {
            if (amount < 0)
                continue;

            var curValue = GetCurrencyTotal(cur, amount);
            total += curValue;
        }

        if (total > stored)
            return 0;

        return total;
    }

    /// <summary>
    /// Helper to get the total amount from currencies
    /// Does not accept negative values.
    /// </summary>
    /// <param name="currencies">The currencies to check against</param>
    /// <returns></returns>
    private int GetTotalFromCurrencies(Dictionary<ProtoId<EconomyCurrencyPrototype>, int> currencies)
    {
        var total = 0;
        foreach (var (cur, amount) in currencies)
        {
            if (amount < 0)
                continue;

            var curValue = GetCurrencyTotal(cur, amount);
            total += curValue;
        }

        return total;
    }

    #endregion
}

/// <summary>
/// Enum that defines a transaction type.
/// I have commented the definitions below to explain what each type does.
/// </summary>
public enum EconomyTransaction : byte
{
    Withdraw,   // Taking money out of an account
    Deposit,    // Putting money in an account
}
