using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Oaths;

/// <summary>
/// Component used on the brain to define what Oath they are aligned to.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState(fieldDeltas: true)]
public sealed partial class OathComponent : Component
{
    /// <summary>
    /// The oath this entity belongs to.
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<OathPrototype> Oath = "Oathless";

    /// <summary>
    /// Whether the effects have been run before on this brain before.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool HasRunEffects;
}
