using System.Numerics;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Timing;

namespace Content.Oathlord.Shared.Hitbox;

/// <summary>
/// System that handles custom behaviour for weapon hitboxes.
///
/// A hitbox is just a Box2 (although we will probably support Arcs later...)
/// since it's likely faster than spawning entities with fixtures (am I coping?).
/// </summary>
public sealed partial class HitboxWeaponSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private DamageableSystem _damageable = default!;

    private HashSet<Entity<DamageableComponent>> _damage = new();

    public readonly List<Box2Rotated> ActiveHitboxes = new();

    /// <summary>
    /// After how many seconds should we reset to the original hitbox. TODO: Cvar
    /// </summary>
    private TimeSpan _resetTimer = TimeSpan.FromSeconds(2);

    public override void Initialize()
    {
        base.Initialize();

        // SubscribeLocalEvent<HitboxWeaponComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<HitboxWeaponComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnMeleeHit(Entity<HitboxWeaponComponent> ent, ref MeleeHitEvent args)
    {
        // We stop the rest of melee system's behavior here, since we do our own
        args.Handled = true;

        var xform = Transform(args.User);

        // Quick explanation:
        // we take the coordinates of the user and the click location,
        // then we rotate the hitbox towards the click location, while maintaining the correct rotation
        // (say, if user is facing up [-180 angle] then the hitbox should be up too, but it should also be rotated by the click location)
        //
        // After that, we can also specify a hitbox rotation, so we can emulate things like a slash attack
        var coords = _transform.GetWorldPosition(xform);
        var worldRot = _transform.GetWorldRotation(xform);
        var clickLoc = GetCoordinates(args.ClickLocation);
        var clickPos = _transform.ToMapCoordinates(clickLoc).Position;

        var dirToClick = clickPos - coords;
        var clickAngle = dirToClick != Vector2.Zero ? dirToClick.ToWorldAngle() : worldRot;

        var nextHitbox = ent.Comp.CurrentHitboxIndex + 1;
        if (nextHitbox >= ent.Comp.Hitboxes.Count)
        {
            ent.Comp.CurrentHitboxIndex = -1; // Reset if we reached the final hitbox
            Dirty(ent);
            return;
        }

        // TODO: FIND OUT HOW TO DO THIS!

        var currentHitbox = ent.Comp.Hitboxes[nextHitbox];

        // We rotate the offset to face where the user is looking, otherwise it'd be a fixed offset
        var rotatedOffset = Vector2.Zero;
        if (currentHitbox.Offset != Vector2.Zero)
        {
            rotatedOffset = clickAngle.RotateVec(currentHitbox.Offset);
        }

        var worldPos = coords + rotatedOffset;
        var shape = Box2.CenteredAround(worldPos, currentHitbox.HitboxSize);
        var rotatedShape = new Box2Rotated(shape, clickAngle + currentHitbox.Rotation, worldPos);

        ActiveHitboxes.Add(rotatedShape);

        var activeComp = EnsureComp<ActiveHitboxWeaponComponent>(ent.Owner);
        activeComp.CurrentHitboxIndex = ent.Comp.CurrentHitboxIndex;
        activeComp.NextReset = _resetTimer + _timing.CurTime;
    }
}
