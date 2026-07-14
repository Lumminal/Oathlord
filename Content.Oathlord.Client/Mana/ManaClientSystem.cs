using Content.Oathlord.Shared.Mana;
using Content.Shared.FixedPoint;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Oathlord.Client.Mana;

public sealed partial class ManaClientSystem : ManaSystem
{
    [Dependency] private IPlayerManager _player = default!;

    public event EventHandler<FixedPoint2>? SyncMana;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ManaUserComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
    }

    private void OnPlayerAttached(Entity<ManaUserComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        if (_player.LocalEntity == ent.Owner)
            SyncMana?.Invoke(this, ent.Comp.CurrentMana);
    }

    protected override void UpdateHud(Entity<ManaUserComponent> ent)
    {
        if (_player.LocalEntity == ent.Owner)
            SyncMana?.Invoke(this, ent.Comp.CurrentMana);
    }
}
