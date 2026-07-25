using System.Linq;
using Content.Oathlord.Shared.Economy.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Economy.Systems;

public sealed partial class OathlordEconomySystem
{
    /// <summary>
    /// Caching the values of the currencies, because economy is gonna use it a lot
    /// (and we know that they won't change since they are prototypes)
    /// </summary>
    private Dictionary<ProtoId<EconomyCurrencyPrototype>, int> _allCurrencyValues = new();

    private void InitializeCurrency()
    {
        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnReload);

        LoadValues();
    }

    private void OnReload(PrototypesReloadedEventArgs args)
    {
        if (!args.WasModified<EconomyCurrencyPrototype>())
            return;

        LoadValues();
    }

    private void LoadValues()
    {
        _allCurrencyValues.Clear();
        foreach (var proto in _proto.EnumeratePrototypes<EconomyCurrencyPrototype>())
        {
            _allCurrencyValues.Add(proto, proto.Value);
        }
    }

    /// <summary>
    /// Gets the total amount of the currency, by multiplying the currency's value with a specified amount.
    /// </summary>
    public int GetCurrencyTotal(ProtoId<EconomyCurrencyPrototype> currencyProto, int amount)
    {
        if (!_proto.Resolve(currencyProto, out var currency))
            return 0;

        return currency.Value * amount;
    }

    /// <summary>
    /// Gets the value of a currency prototype
    /// </summary>
    public int GetCurrencyValue(ProtoId<EconomyCurrencyPrototype> currencyProto)
    {
        if (!_allCurrencyValues.TryGetValue(currencyProto, out var value))
            return 0;

        return value;
    }
}
