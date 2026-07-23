using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Economy.Components;

/// <summary>
/// Component that is used for objects, to allow withdrawing and depositing money via UI
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EconomyMachineComponent : Component
{

}

[Serializable, NetSerializable]
public sealed class EconomyDepositMessage : BoundUserInterfaceMessage
{
    /// <summary>
    /// The entity the amount is getting deposited to
    /// </summary>
    public NetEntity? DepositEntity;

    /// <summary>
    /// The amount to deposit to the entity
    /// </summary>
    public int Amount;

    public EconomyDepositMessage(NetEntity? depositEntity, int amount)
    {
        DepositEntity = depositEntity;
        Amount = amount;
    }
}

[Serializable, NetSerializable]
public enum EconomyMachineUiKey : byte
{
    Key,
}
