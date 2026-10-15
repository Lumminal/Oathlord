using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.CharacterAttributes;

public sealed partial class CharacterAttributesSystem
{
    /// <summary>
    /// Adds an attribute to the container of the entity
    /// </summary>
    /// <param name="ent">The entity</param>
    /// <param name="attribute">The attribute to add</param>
    public void AddAttribute(Entity<CharacterAttributesComponent?> ent, [ForbidLiteral] EntProtoId attribute)
    {
        if (!_characterAttrQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        if (!AllAttributes.Contains(attribute))
            return;

        foreach (var entity in ent.Comp.Container.ContainedEntities)
        {
            // Don't allow duplicate attributes to go in...
            if (Prototype(entity) is not { } proto || proto == attribute)
            {
                Log.Warning($"Tried to add an attribute {attribute.Id} that already " +
                            $"exists on the entity {ToPrettyString(ent)}");
                return;
            }
        }

        PredictedTrySpawnInContainer(
            protoName: attribute,
            containerUid: ent,
            containerId: CharacterAttributesComponent.ContainerId,
            uid: out _);
    }
}
