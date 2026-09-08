using Robust.Shared.GameStates;

namespace Content.Oathlord.Shared.Blacksmith.Hammer;

/// <summary>
/// Component used on entities that allow the user to work on Anvils.
/// If you're not holding one of those, then look for another job, pal.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HammerComponent : Component;
