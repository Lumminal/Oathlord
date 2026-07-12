using System.Numerics;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Oathlord.Shared.Hitbox;

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

        var coords = _transform.GetWorldPosition(xform);
        var worldRot = _transform.GetWorldRotation(xform);

        var first = ent.Comp.Hitboxes[0];

        // We rotate the offset to face where the user is looking, otherwise it'd be a fixed offset
        var rotatedOffset = Vector2.Zero;
        if (first.Offset != Vector2.Zero)
        {
            rotatedOffset = worldRot.RotateVec(first.Offset);
        }

        if (args.Direction is not { } direction)
            return;

        var worldPos = coords + rotatedOffset;
        var shape = Box2.CenteredAround(worldPos, first.HitboxSize);
        var rotatedShape = new Box2Rotated(shape, worldRot + first.Rotation + direction.ToAngle(), worldPos);

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
