using Content.Oathlord.Shared.Blacksmith.Anvil.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Blacksmith.Anvil;

/// <summary>
/// Component used on entities (usually structures) to give them the ability to work on <see cref="MetalWorkableComponent"/> entities.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AnvilComponent : Component
{
    /// <summary>
    /// How many workables we can have in this anvil at a time
    /// </summary>
    [DataField]
    public int AllowedWorkables = 2;

    /// <summary>
    /// The recipe that was selected to be worked on
    /// </summary>
    [DataField]
    public ProtoId<AnvilRecipePrototype>? SelectedRecipe;
}

[Serializable, NetSerializable]
public enum AnvilUiKey : byte
{
    Key,
}
