using Content.Shared.FixedPoint;

namespace Content.Oathlord.Shared.Mana;

/// <summary>
/// Public API for anything mana-related.
/// </summary>
public abstract partial class ManaSystem : EntitySystem
{
    [Dependency] private EntityQuery<ManaUserComponent> _manaQuery = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ManaUserComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<ManaUserComponent> ent, ref MapInitEvent args)
    {
        UpdateHud(ent);
    }

    public FixedPoint2 GetMana(Entity<ManaUserComponent?> ent)
    {
        if (!_manaQuery.Resolve(ent.Owner, ref ent.Comp))
            return 0;

        return ent.Comp.CurrentMana;
    }

    public FixedPoint2 GetMaxMana(Entity<ManaUserComponent?> ent)
    {
        if (!_manaQuery.Resolve(ent.Owner, ref ent.Comp))
            return 0;

        return ent.Comp.MaxMana;
    }

    public void AdjustMana(Entity<ManaUserComponent?> ent, FixedPoint2 amount)
    {
        if (!_manaQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        ent.Comp.CurrentMana = FixedPoint2.Clamp(ent.Comp.CurrentMana + amount, 0, ent.Comp.MaxMana);
        Dirty(ent);

        UpdateHud((ent.Owner, ent.Comp));
    }

    protected virtual void UpdateHud(Entity<ManaUserComponent> ent) { }
}
