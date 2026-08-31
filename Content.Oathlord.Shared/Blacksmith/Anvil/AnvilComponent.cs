using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Blacksmith.Anvil;

/// <summary>
/// Component used on entities (usually structures) to give them the ability to work on <see cref="MetalWorkableComponent"/> entities.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AnvilComponent : Component
{
    // todo:
    // 1. Not all anvils can work with all metals
    // 2. Container
    // 3. Minigame implementation from tfc
}

[Serializable, NetSerializable]
public enum AnvilUiKey : byte
{
    Key,
}
