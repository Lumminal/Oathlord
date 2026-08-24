using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Spellcasting.Components;

/// <summary>
/// Component that handles spellcasting. It holds all learned spells, and active spells.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState(fieldDeltas: true)]
public sealed partial class SpellsComponent : Component
{
    /// <summary>
    /// All the spells this entity has learned, and therefore will appear in the UI, so far.
    /// Does not include active spells.
    /// List so we can preserve order.
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<EntityUid> LearnedSpells = new();

    /// <summary>
    /// The index of the current selected spell, to be used with <see cref="Slots"/>
    /// </summary>
    [DataField, AutoNetworkedField]
    public int ActiveSpell;

    /// <summary>
    /// The active spell slots we have. Active spells can be activated by the user, as opposed to <see cref="LearnedSpells"/>.
    /// Same as before, list for preserving order
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<EntityUid> Slots = new();

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

    // todo: seperate it to different component like actions...?
    public const string ContainerId = "spells";

    [ViewVariables]
    public Container Container = default!;
}

[ByRefEvent]
public record struct RequestSpellTransferEvent(EntityUid Spell, SpellTransfer Type);

[Serializable, NetSerializable]
public sealed class SpellTransferEvent(NetEntity spell, SpellTransfer type) : EntityEventArgs
{
    public NetEntity Spell = spell;
    public SpellTransfer Type = type;
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
