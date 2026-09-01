using Content.Oathlord.Shared.Blacksmith.Anvil.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Blacksmith.Anvil;

/// <summary>
/// Component used on materials to make them valid to be used on <see cref="AnvilComponent"/> entities.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class MetalWorkableComponent : Component
{
    /// <summary>
    /// Defines what this metal can be turned into.
    /// </summary>
    [DataField] // todo: required
    public List<ProtoId<AnvilRecipePrototype>> Outputs = new();

    /// <summary>
    /// The amount that has been worked on this metal, towards the selected recipe
    /// </summary>
    [DataField, AutoNetworkedField]
    public int WorkedAmount;
}
