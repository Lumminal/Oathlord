using Content.Shared.FixedPoint;

namespace Content.Oathlord.Shared.Mana;

/// <summary>
/// Public API for anything mana-related.
/// </summary>
public abstract partial class ManaSystem : EntitySystem
{
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
        if (!Resolve(ent.Owner, ref ent.Comp))
            return 0;

        return ent.Comp.CurrentMana;
    }

    protected virtual void UpdateHud(Entity<ManaUserComponent> ent) { }
}
