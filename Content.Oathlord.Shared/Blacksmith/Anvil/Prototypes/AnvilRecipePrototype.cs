using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Blacksmith.Anvil.Prototypes;

/// <summary>
/// Prototype used for recipes of <see cref="MetalWorkableComponent"/> entities.
/// </summary>
[Prototype]
public sealed partial class AnvilRecipePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set;  } = default!;

    /// <summary>
    /// How many of the same metal is required for this recipe
    /// </summary>
    [DataField]
    public int MetalAmount = 1;

    /// <summary>
    /// How much work it is required for this recipe.
    /// This should be a number between 0 and 100
    /// </summary>
    [DataField]
    public int WorkRequired = 50;

    /// <summary>
    /// What will result from this recipe
    /// </summary>
    [DataField]
    public EntProtoId Result;
}
