using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Blacksmith.Anvil;

/// <summary>
/// Component used on materials to make them valid to be used on <see cref="AnvilComponent"/> entities.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class MetalWorkableComponent : Component
{
    /// <summary>
    /// Defines what this metal can be turned into.
    /// </summary>
    /// <remarks>I don't expect the metal to get more outputs, that's why it's an array</remarks>
    [DataField(required: true)]
    public MetalOutputData[] Outputs = default!;
}

/// <summary>
/// Data that holds the amount required for a metal to meet the results.
/// <example>
/// I have 1 iron bar on the anvil, this means this bar can be turned into X results.
/// When I put 1 extra bar on the anvil (making us 2 iron bars), this changes the output to Y things.
///
/// In this case, 2 iron bars could be turned into a double ingot, but 1 bar alone can't do that.
/// </example>
/// </summary>
/// <param name="Amount">How many metals of this type does it take, for the results to show up</param>
/// <param name="Results">What entities this metal can be turned into</param>
[DataRecord, Serializable, NetSerializable]
public partial record struct MetalOutputData(int Amount, EntityTableSelector Results);
