using System.Numerics;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Oathlord.Shared.Hitbox;

/// <summary>
/// System that handles custom behaviour for weapon hitboxes.
///
/// A hitbox is just a Box2 (although we will probably support Arcs later...)
/// since it's likely faster than spawning entities with fixtures (am I coping?).
/// </summary>
public sealed partial class HitboxWeaponSystem : EntitySystem
{
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private DamageableSystem _damageable = default!;

    private HashSet<Entity<DamageableComponent>> _damage = new();

    public readonly List<Box2Rotated> ActiveHitboxes = new();

    public override void Initialize()
    {
        base.Initialize();

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

        var first = ent.Comp.Hitboxes[0];

        // We rotate the offset to face where the user is looking, otherwise it'd be a fixed offset
        var rotatedOffset = Vector2.Zero;
        if (first.Offset != Vector2.Zero)
        {
            rotatedOffset = clickAngle.RotateVec(first.Offset);
        }

        var worldPos = coords + rotatedOffset;
        var shape = Box2.CenteredAround(worldPos, first.HitboxSize);
        var rotatedShape = new Box2Rotated(shape, clickAngle + first.Rotation, worldPos);

        ActiveHitboxes.Add(rotatedShape);

        _damage.Clear();
        _lookup.GetEntitiesIntersecting(xform.MapID, rotatedShape, _damage);
        foreach (var entity in _damage)
        {
            if (entity.Owner == args.User)
                continue;

            _damageable.ChangeDamage(entity.AsNullable(), first.Damage, true);
        }
    }
}
