using System.Numerics;
using Content.Shared.Damage;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.Hitbox;

/// <summary>
/// A component that makes weapons spawn hitboxes, instead of normal attacks.
/// TODO: Reset timer: after how many seconds should the hitbox reset to the first one?
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class HitboxWeaponComponent : Component
{
    /// <summary>
    /// The list of hitboxes this weapon can perform, in an ordered sequence.
    ///
    /// Each hitbox has a built-in lifetime, that means the user must wait for the hitbox's lifetime
    /// to end before commiting the next hitbox in line.
    /// </summary>
    [DataField(required: true)]
    public List<HitboxData> Hitboxes;

    /// <summary>
    /// The current active hitbox.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int CurrentHitboxIndex = -1;
}

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class ActiveHitboxWeaponComponent : Component
{
    /// <summary>
    /// The current active hitbox.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int CurrentHitboxIndex;

    [DataField, AutoNetworkedField]
    public TimeSpan NextHitbox;

    [DataField, AutoNetworkedField]
    public TimeSpan NextReset;
}

/// <summary>
/// Holds data related to a hitbox
/// </summary>
[DataRecord, Serializable, NetSerializable]
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
    public DamageSpecifier? Damage;

    /// <summary>
    /// How long this hitbox lasts
    /// </summary>
    [DataField(required: true)]
    public TimeSpan Lifetime;
}
