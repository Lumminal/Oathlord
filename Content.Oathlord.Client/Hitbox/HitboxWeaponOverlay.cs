using Content.Oathlord.Shared.Hitbox;
using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Oathlord.Client.Hitbox;

public sealed partial class HitboxWeaponOverlay : Overlay
{
    private HitboxWeaponSystem _hitbox;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    public HitboxWeaponOverlay(IEntityManager entManager)
    {
        _hitbox = entManager.System<HitboxWeaponSystem>();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (args.Viewport.Eye == null)
            return;

        var worldHandle = args.WorldHandle;

        foreach (var hitbox in _hitbox.ActiveHitboxes)
        {
            worldHandle.DrawRect(hitbox, Color.Blue);
        }
    }
}
