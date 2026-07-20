using Robust.Shared.GameStates;

namespace Content.Oathlord.Shared.Economy.Components;

/// <summary>
/// Component that is applied to the player once they spawn.
/// Allows them to be part of the economy, by initializing a new bank account in the economy system.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EconomyAccountComponent : Component
{
    /// <summary>
    /// The current amount of currency this entity has stored in the bank.
    /// </summary>
    [DataField]
    public int Stored;
}
