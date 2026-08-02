using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

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
    /// For our purposes, this variable is considered the lowest tier currency (which is copper aka Na).
    /// However, it can be converted into silver and gold via other functions (100 copper = 1 silver, 100 silver = 1 gold).
    /// IMPORTANT: This indicates how much "ownership" the user has over the bank's vault/treasury! It does not act as a mini-bank.
    ///
    /// This is a <see cref="int"/> because float values generally add an extra layer of complexity we don't need.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Stored;

    [DataField, AutoNetworkedField]
    public List<LoanData> Loans = new();
}

[Serializable, NetSerializable, DataRecord]
public partial record struct LoanData
{
    [DataField]
    public TimeSpan DueTime;

    [DataField]
    public int Amount;

    [DataField]
    public bool Paid;

    [DataField]
    public string? Reason;
}
