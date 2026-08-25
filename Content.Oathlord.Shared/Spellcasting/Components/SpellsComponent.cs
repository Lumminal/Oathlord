using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Spellcasting.Components;

/// <summary>
/// Component that handles spellcasting. It holds all learned spells, and active spells.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class SpellsComponent : Component
{
    /// <summary>
    /// The current selected spell
    /// </summary>
    [DataField, AutoNetworkedField]
    public int SelectedSpell;

    /// <summary>
    /// How many slots we currently have
    /// </summary>
    [DataField]
    public int CurrentSlots = 2;

    /// <summary>
    /// How many learned slots we are allowed to have
    /// </summary>
    [DataField]
    public int MaxLearned = 10;

    public const string ContainerId = "spells";

    [ViewVariables]
    public Container Container = default!;
}

[ByRefEvent]
public record struct RequestSpellTransferEvent(EntityUid Spell, SpellTransfer Type, bool Cancelled = false);

[Serializable, NetSerializable]
public sealed class SpellTransferEvent(NetEntity spell, SpellTransfer type, bool cancelled = false) : EntityEventArgs
{
    public NetEntity Spell = spell;
    public SpellTransfer Type = type;
    public bool Cancelled = cancelled;
};

/// <summary>
/// We know there's 2 types of categories in our spellcasting UI. The learned spells, and the active spells.
/// This enum exists to help with transferring from one category to another.
/// </summary>
[NetSerializable, Serializable]
public enum SpellTransfer : byte
{
    Learned,
    Active,
}
