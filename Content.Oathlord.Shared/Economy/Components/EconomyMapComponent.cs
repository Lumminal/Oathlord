using Robust.Shared.GameStates;

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
}
