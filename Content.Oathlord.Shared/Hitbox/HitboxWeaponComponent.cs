using System.Numerics;
using Content.Shared.Damage;
using Robust.Shared.GameStates;

namespace Content.Oathlord.Shared.Hitbox;

/// <summary>
/// A component that makes weapons spawn hitboxes, instead of normal attacks.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HitboxWeaponComponent : Component
{
    [DataField(required: true)]
    public List<HitboxData> Hitboxes;
}

[DataDefinition]
public partial record struct HitboxData
{
    /// <summary>
    /// The size of the hitbox
    /// </summary>
    [DataField]
    public Vector2 HitboxSize;

    /// <summary>
    /// The offset of the hitbox, relevant to the attacker
    /// </summary>
    [DataField]
    public Vector2 Offset;

    /// <summary>
    /// The rotation of the hitbox
    /// </summary>
    [DataField]
    public Angle Rotation;

    /// <summary>
    /// How much damage the hitbox deals to anyone inside it
    /// </summary>
    [DataField]
    public DamageSpecifier Damage;
}
