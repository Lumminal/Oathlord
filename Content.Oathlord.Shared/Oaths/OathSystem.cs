using Content.Shared.Body;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Oathlord.Shared.Oaths;

/// <summary>
/// Oaths are beliefs, similar to Patrons/Gods.
/// They are a little more complex. todo: expand
/// </summary>
public sealed partial class OathSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private BodySystem _body = default!;
    [Dependency] private SharedEntityEffectsSystem _effects = default!;
    [Dependency] private EntityQuery<OathComponent> _oathQuery = default!;

    [SubscribeLocalEvent]
    public void OnOrganInserted(Entity<OathComponent> ent, ref OrganGotInsertedEvent args)
    {
        if (_timing.ApplyingState)
            return;

        // Only run the effects once, on round-start.
        if (ent.Comp.HasRunEffects)
            return;

        var effects = GetEffects(ent.Comp.Oath);
        _effects.ApplyEffects(args.Target, effects);

        ent.Comp.HasRunEffects = true;
        DirtyField(ent.AsNullable(), nameof(OathComponent.HasRunEffects));
    }

    /// <summary>
    /// Returns the oath active in the entity's brain
    /// </summary>
    /// <param name="uid">The entity</param>
    /// <returns>Null if the oath was not found</returns>
    public ProtoId<OathPrototype>? GetOath(EntityUid uid)
    {
        // Return early if it's a brain
        if (_oathQuery.TryComp(uid, out var oath))
            return oath.Oath;

        var organs = _body.EnumerateOrgans<OathComponent>(uid);
        foreach (var (_, _, oathOrgan) in organs)
            return oathOrgan.Oath;

        return null;
    }

    /// <summary>
    /// Sets the oath on a brain
    /// </summary>
    /// <param name="ent">The brain</param>
    /// <param name="oath">The oath to set it to</param>
    public void SetOath(Entity<OathComponent?> ent, ProtoId<OathPrototype> oath) =>
        SetOath(ent, oath, null);

    /// <summary>
    /// Sets the oath on a brain
    /// </summary>
    /// <param name="ent">The brain</param>
    /// <param name="oath">The oath to set it to</param>
    /// <param name="user">The body entity</param>
    /// <param name="runEffects">If user is non-null, whether we should run the entity effects when setting the oath</param>
    public void SetOath(Entity<OathComponent?> ent, ProtoId<OathPrototype> oath, EntityUid? user, bool runEffects = false)
    {
        if (!_oathQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        ent.Comp.Oath = oath;
        DirtyField(ent, nameof(OathComponent.Oath));

        if (!runEffects || user is not { } usr)
            return;

        var effects = GetEffects(ent.Comp.Oath);
        _effects.ApplyEffects(usr, effects);
    }

    /// <summary>
    /// Returns the <see cref="OathPrototype.Effects"/>
    /// </summary>
    /// <param name="oath">The oath prototype</param>
    /// <returns>Empty if prototype was not resolved</returns>
    public EntityEffect[] GetEffects(ProtoId<OathPrototype> oath)
    {
        if (!ProtoMan.Resolve(oath, out var oathProto))
            return [];

        return oathProto.Effects;
    }
}
