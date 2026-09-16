using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Oaths;

/// <summary>
/// Prototype for oaths, which are basically "alignments" which give special effects to the users.
/// </summary>
[Prototype]
public sealed partial class OathPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Effects that will run on the user, when an oath has been chosen
    /// </summary>
    [DataField(required: true)]
    public EntityEffect[] Effects = default!;
}
