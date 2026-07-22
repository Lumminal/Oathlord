using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Economy.Components;

/// <summary>
/// Component that is used for objects, to allow withdrawing and depositing money via UI
/// </summary>
public sealed partial class EconomyMachineComponent : Component
{

}

[Serializable, NetSerializable]
public enum EconomyMachineUiKey : byte
{
    Key,
}
