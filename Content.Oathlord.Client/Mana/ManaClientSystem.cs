using Content.Oathlord.Shared.Mana;
using Content.Shared.FixedPoint;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Oathlord.Client.Mana;

public sealed partial class ManaClientSystem : ManaSystem
{
    [Dependency] private IPlayerManager _player = default!;

    public event EventHandler<(FixedPoint2, FixedPoint2, bool)>? SyncMana;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ManaUserComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<ManaUserComponent, AfterAutoHandleStateEvent>(OnAfterAutoHandle);

        SubscribeLocalEvent<ManaUserComponent, ComponentRemove>(OnRemove);
    }

    private void OnPlayerAttached(Entity<ManaUserComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        if (_player.LocalEntity == ent.Owner)
            SyncMana?.Invoke(this, (ent.Comp.CurrentMana, ent.Comp.MaxMana, ent.Comp.CanUse));
    }

    private void OnAfterAutoHandle(Entity<ManaUserComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        // This is mostly needed for stuff that is being called in the server (like a command)
        UpdateHud(ent);
    }

    private void OnRemove(Entity<ManaUserComponent> ent, ref ComponentRemove args)
    {
        if (_player.LocalEntity == ent.Owner)
            SyncMana?.Invoke(this, (1, 1, false)); // to make the bar look "unavailable" in the ui
    }

    protected override void UpdateHud(Entity<ManaUserComponent> ent)
    {
        if (_player.LocalEntity == ent.Owner)
            SyncMana?.Invoke(this, (ent.Comp.CurrentMana, ent.Comp.MaxMana, ent.Comp.CanUse));
    }
}
