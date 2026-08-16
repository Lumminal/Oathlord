using Content.Oathlord.Shared.Spellcasting.Components;
using Robust.Shared.Containers;

namespace Content.Oathlord.Shared.Spellcasting.Systems;

/// <summary>
/// Handles anything related to spellcasting, and provides a public api
/// </summary>
public sealed partial class SpellcastingSystem : EntitySystem
{
    [Dependency] private SharedContainerSystem _container = default!;

    [SubscribeLocalEvent]
    public void OnMapInit(Entity<SpellsComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.Container = _container.EnsureContainer<Container>(ent, SpellsComponent.ContainerId);

        // debug shit
        var debug = Spawn("SpellDebug");
        _container.Insert(debug, ent.Comp.Container);

        ent.Comp.LearnedSpells.Add(debug);
        DirtyField(ent.AsNullable(), nameof(SpellsComponent.LearnedSpells));
    }
}
