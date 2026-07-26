using Content.Oathlord.Shared.Economy.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Economy.Components;

/// <summary>
/// Component applied to the map to store information about accounts, and anything related to the economy.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class EconomyMapComponent : Component
{
    /// <summary>
    /// All the active accounts in this economy
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<EntityUid> ActiveAccounts = new();

    /// <summary>
    /// The currencies we currently have stored
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<EconomyCurrencyPrototype>, int> StoredCurrencies = new();

    /// <summary>
    /// Because we may want to look this value often (for UIs), we cache it here.
    /// Use <see cref="GetTotalEconomyStored"/> method instead for getting this value.
    /// </summary>
    [AutoNetworkedField]
    public int TotalStored;
}
