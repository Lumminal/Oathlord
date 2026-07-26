using Content.Oathlord.Shared.Economy.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Economy.Components;

/// <summary>
/// Component that is used for objects, to allow withdrawing and depositing money via UI
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EconomyMachineComponent : Component;

// TODO: withdraw and deposit message have same variables
[Serializable, NetSerializable]
public sealed class EconomyDepositMessage : BoundUserInterfaceMessage
{
    /// <summary>
    /// The entity the amount is getting deposited to
    /// </summary>
    public NetEntity? DepositEntity;

    /// <summary>
    /// The amount to deposit to the account
    /// </summary>
    public int Amount;

    public EconomyDepositMessage(NetEntity? depositEntity, int amount)
    {
        DepositEntity = depositEntity;
        Amount = amount;
    }
}

[Serializable, NetSerializable]
public sealed class EconomyWithdrawMessage : BoundUserInterfaceMessage
{
    /// <summary>
    /// The entity the amount is getting withdrawn off
    /// </summary>
    public NetEntity? WithdrawEntity;

    /// <summary>
    /// The amount of currencies to withdraw from this account.
    /// </summary>
    public Dictionary<ProtoId<EconomyCurrencyPrototype>, int> ToWithdraw;

    public EconomyWithdrawMessage(NetEntity? withdrawEntity,  Dictionary<ProtoId<EconomyCurrencyPrototype>, int> toWithdraw)
    {
        WithdrawEntity = withdrawEntity;
        ToWithdraw = toWithdraw;
    }
}

[Serializable, NetSerializable]
public enum EconomyMachineUiKey : byte
{
    Key,
}
