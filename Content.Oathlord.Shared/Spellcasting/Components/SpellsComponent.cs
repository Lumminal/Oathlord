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
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<EntityUid> LearnedSpells = new();

    /// <summary>
    /// The current selected spell
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? ActiveSpell;

    /// <summary>
    /// The spell slots we have. Each slot holds a spell
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<EntityUid> Slots = new();

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
}

[Serializable, NetSerializable]
public enum SpellsUiKey : byte
{
    Key,
}
