using Robust.Shared.GameStates;

namespace Content.Oathlord.Shared.Spellcasting.Components;

/// <summary>
/// Component used on actions to mark it as a spell
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class SpellComponent : Component
{
    /// <summary>
    /// Whether this spell is active, or inactive on the user's spell container
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Active;
};
