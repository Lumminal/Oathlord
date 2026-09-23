using Content.Oathlord.Common.Oaths;
using Content.Shared.Body;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Oaths;

/// <summary>
/// Oaths are beliefs, similar to Patrons/Gods.
/// They are a little more complex. todo: expand
/// </summary>
public abstract partial class OathSystem : CommonOathSystem
{
    [Dependency] private BodySystem _body = default!;
    [Dependency] private SharedEntityEffectsSystem _effects = default!;

    [Dependency] private EntityQuery<OathComponent> _oathQuery = default!;

    [ViewVariables]
    public List<ProtoId<OathPrototype>> AllOaths = new();

    public override void Initialize()
    {
        base.Initialize();

        LoadOaths();
    }

    [SubscribeLocalEvent]
    public void OnProtoReload(PrototypesReloadedEventArgs args)
    {
        if (!args.WasModified<OathPrototype>())
            return;

        LoadOaths();
    }

    /// <summary>
    /// Returns the oath active in the entity's brain
    /// </summary>
    /// <param name="uid">The user</param>
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
    public void SetOath(Entity<OathComponent?> ent, [ForbidLiteral] ProtoId<OathPrototype> oath) =>
        SetOath(ent, oath, null);

    /// <summary>
    /// Sets the oath on a brain
    /// </summary>
    /// <param name="ent">The brain</param>
    /// <param name="oath">The oath to set it to</param>
    /// <param name="user">The body entity</param>
    /// <param name="runEffects">If user is non-null, whether we should run the entity effects when setting the oath</param>
    public void SetOath(Entity<OathComponent?> ent, [ForbidLiteral] ProtoId<OathPrototype> oath, EntityUid? user, bool runEffects = true)
    {
        if (!_oathQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        ent.Comp.Oath = oath;
        Dirty(ent);

        if (!runEffects || user is not { } usr)
            return;

        ApplyOathEffects(usr, ent.Comp.Oath);
    }

    /// <summary>
    /// Sets the oath on a brain
    /// </summary>
    /// <param name="user">The user entity</param>
    /// <param name="oath">The oath to apply</param>
    /// <param name="runEffects">Whether to run effects, or not</param>
    public void SetOath(EntityUid user, [ForbidLiteral] ProtoId<OathPrototype> oath, bool runEffects = true)
    {
        // It's a brain...
        if (_oathQuery.TryComp(user, out var oathComp))
        {
            SetOath((user, oathComp), oath);
            return;
        }

        var organs = _body.EnumerateOrgans<OathComponent>(user);
        foreach (var (oathUid, _, oathOrgan) in organs)
        {
            // Apply to first found only
            SetOath((oathUid, oathOrgan), oath, user, runEffects);
            return;
        }
    }

    /// <summary>
    /// Returns the <see cref="OathPrototype.Effect"/>
    /// </summary>
    /// <param name="oath">The oath prototype</param>
    /// <returns>Empty if prototype was not resolved</returns>
    public ProtoId<EntityEffectPrototype>? GetEffects([ForbidLiteral] ProtoId<OathPrototype> oath)
    {
        if (!ProtoMan.Resolve(oath, out var oathProto))
            return null;

        return oathProto.Effect;
    }

    public override void ApplyOath(EntityUid target, [ForbidLiteral] ProtoId<OathPrototype> oath)
    {
        SetOath(target, oath);
    }

    /// <summary>
    /// Helper to apply oath effects
    /// </summary>
    /// <param name="target">The target to apply the effects to</param>
    /// <param name="oath">The oath</param>
    private void ApplyOathEffects(EntityUid target, ProtoId<OathPrototype> oath)
    {
        var effect = GetEffects(oath);
        if (effect is not { } entEffect)
            return;

        _effects.TryApplyEffect(target, entEffect);
    }

    private void LoadOaths()
    {
        AllOaths.Clear();
        foreach (var oath in ProtoMan.EnumeratePrototypes<OathPrototype>())
        {
            AllOaths.Add(oath);
        }
    }
}
