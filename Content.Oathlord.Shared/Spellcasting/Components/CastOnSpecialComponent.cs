using Robust.Shared.GameStates;

namespace Content.Oathlord.Shared.Spellcasting.Components;

/// <summary>
/// Component that casts the active selected spell when an item does an item special interaction
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class CastOnSpecialComponent : Component;
