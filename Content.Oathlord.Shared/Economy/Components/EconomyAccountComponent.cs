using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Oathlord.Shared.Economy.Components;

/// <summary>
/// Component that is applied to the player once they spawn.
/// Allows them to be part of the economy, by initializing a new bank account in the economy system.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class EconomyAccountComponent : Component
{
    /// <summary>
    /// The current amount of currency this entity has stored in the bank.
    /// For our purposes, this variable is considered the lowest tier currency (which is copper Nar).
    /// However, it can be converted into silver and gold via other functions (100 copper = 1 silver, 100 silver = 1 gold).
    ///
    /// This is a <see cref="int"/> because float values generally add an extra layer of complexity we don't need.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Stored;
}
