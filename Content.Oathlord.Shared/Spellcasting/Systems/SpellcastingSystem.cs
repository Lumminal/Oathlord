using Content.Oathlord.Shared.Spellcasting.Components;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Oathlord.Shared.Spellcasting.Systems;

/// <summary>
/// Handles anything related to spellcasting, and provides a public api
///
/// Spells are wrappers around actions (for flexibility purposes), but are bound by their own rules.
///
/// Some of the important ones being:
/// - You can have only 1 spell active at a time to use
/// - Spells usually require a catalyst in order to be casted (usually an item on the user's hand)
/// - You have a limited amount of spells you can switch to
/// - You have a limited amount of spells you can learn
///
/// The purpose of spells is to eliminate the management of tons of actions on the user's hotbar,
/// and instead make them be usable only within certain contexts.
/// </summary>
public abstract partial class SpellcastingSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedContainerSystem _container = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedActionsSystem _actions = default!;

    [Dependency] private EntityQuery<SpellsComponent> _spellsQuery = default!;
    [Dependency] private EntityQuery<SpellComponent> _spellQuery = default!;
    [Dependency] private EntityQuery<WorldTargetActionComponent> _worldActionQuery = default!;

    /// <summary>
    /// List of every spell prototype loaded in the game
    /// </summary>
    [ViewVariables]
    public List<EntProtoId> AllSpells = new();

    public override void Initialize()
    {
        base.Initialize();

        LoadSpells();
    }

    [SubscribeLocalEvent]
    public void OnCompInit(Entity<SpellsComponent> ent, ref ComponentInit args)
    {
        ent.Comp.Container = _container.EnsureContainer<Container>(ent, SpellsComponent.ContainerId);
    }

    [SubscribeLocalEvent]
    public void OnShutdown(Entity<SpellsComponent> ent, ref ComponentShutdown args)
    {
        if (_timing.ApplyingState && ent.Comp.NetSyncEnabled)
            return;

        _container.ShutdownContainer(ent.Comp.Container);
    }

    // todo: mindadded and mindremoved support for spells

    [SubscribeLocalEvent]
    public void OnMapInit(Entity<SpellsComponent> ent, ref MapInitEvent args)
    {
        // debug shit
        AddSpell(ent.AsNullable(), "SpellDebug");
        AddSpell(ent.AsNullable(), "SpellDebug2");
        AddSpell(ent.AsNullable(), "SpellDebug3");
    }

    [EventSubscription]
    public void OnTransfer(SpellTransferEvent msg, EntitySessionEventArgs args)
    {
        var attached = args.SenderSession.AttachedEntity;
        if (attached is not { } entity)
            return;

        var spell = GetEntity(msg.Spell);
        var transferTo = msg.Type;

        if (TryTransferSpell(entity, spell, transferTo))
            return;

        msg.Cancelled = true;
        _popup.PopupCursor($"You can not transfer this spell to the {transferTo.ToString()} category", entity, PopupType.MediumCaution);
    }

    [SubscribeLocalEvent]
    public void OnPrototypesReload(PrototypesReloadedEventArgs args)
    {
        if (!args.WasModified<EntityPrototype>())
            return;

        LoadSpells();
    }

    #region Public API

    /// <summary>
    /// Tries to transfer a spell from one category to another.
    ///
    /// You probably shouldn't be using this, as it's for UI purposes mostly, but don't tell me I didn't warn you
    /// </summary>
    /// <param name="ent">The spellcaster</param>
    /// <param name="spell">The spell to transfer</param>
    /// <param name="spellTransferType">In which category to transfer to; either learned spells or active spells</param>
    /// <returns>False if the transferring was canceled, true otherwise</returns>
    public bool TryTransferSpell(Entity<SpellsComponent?> ent, EntityUid spell, SpellTransfer spellTransferType)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return false;

        if (!_spellQuery.HasComp(spell) || !ent.Comp.Container.Contains(spell))
            return false;

        var activeSpells = GetSpells(ent, true);
        var learnedSpells = GetSpells(ent, false);

        switch (spellTransferType)
        {
            case SpellTransfer.Active:
            {
                if (activeSpells.Count >= ent.Comp.CurrentSlots)
                    return false;

                break;
            }
            case SpellTransfer.Learned:
            {
                if (learnedSpells.Count >= ent.Comp.MaxLearned)
                    return false;

                break;
            }
        }

        TransferSpell(ent, spell, spellTransferType);
        return true;
    }

    /// <summary>
    /// Transfers a spell from one category to another
    /// </summary>
    /// <param name="ent">The spellcaster</param>
    /// <param name="spell">The spell to transfer</param>
    /// <param name="spellTransferType">In which category to transfer to; either learned spells or active spells</param>
    public void TransferSpell(Entity<SpellsComponent?> ent, EntityUid spell, SpellTransfer spellTransferType)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        switch (spellTransferType)
        {
            case SpellTransfer.Active:
            {
                SetActive(spell, true);
                break;
            }
            case SpellTransfer.Learned:
            {
                SetActive(spell, false);
                break;
            }
        }
    }

    /// <summary>
    /// Casts the active selected spell of the entity
    /// </summary>
    public void CastSpell(Entity<SpellsComponent?> ent, EntityUid? target, EntityCoordinates coords)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        if (GetActiveSpell(ent) is not { } spellEntity || !IsValid(spellEntity) )
            return;

        // this method is pure hell due to actions being hardcoded, but is simple, small and works. I cbf to fix action ancientcode
        _actions.PerformSpellAction(ent, spellEntity, target, coords);
    }

    /// <summary>
    /// Gets the current selected active spell of the user
    /// </summary>
    public EntityUid? GetActiveSpell(Entity<SpellsComponent?> ent)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return null;

        var activeSpells = GetSpells(ent, true);
        var selectedSpell = ent.Comp.SelectedSpell;

        if (selectedSpell < 0 || selectedSpell >= activeSpells.Count)
            return null;

        return activeSpells[selectedSpell];
    }

    /// <summary>
    /// Adds a spell to the entity's learned spells category
    /// </summary>
    /// <param name="ent">The entity to add the spell to</param>
    /// <param name="spell">The spell prototype to add</param>
    /// <param name="force">Whether to add the spell, without checking whether we already have it</param>
    /// <returns>The spell entity</returns>
    public EntityUid? AddSpell(Entity<SpellsComponent?> ent, EntProtoId spell, bool force = false) // todo: forbid literal
    {
        // todo: check against AllSpells that the spell is valid
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return null;

        // todo: spells with charges should add to the charges of an existing spell...!!
        if (!force && HasSpell(ent, spell))
            return null;

        var spellSpawn = Spawn(spell);
        InsertSpell(ent, spellSpawn);

        // if insertion failed, the spawned spell must be deleted
        if (!ent.Comp.Container.Contains(spellSpawn))
        {
            PredictedQueueDel(spellSpawn);
            return null;
        }

        return spellSpawn;
    }

    /// <summary>
    /// Inserts a spell into the learned spells of the user
    /// </summary>
    public void InsertSpell(Entity<SpellsComponent?> ent, EntityUid spell)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        if (_actions.GetAction(spell) is not { } action)
            return;

        action.Comp.AttachedEntity = ent.Owner;
        DirtyField(action.AsNullable(), nameof(ActionComponent.AttachedEntity));

        _container.Insert(spell, ent.Comp.Container);
    }

    /// <summary>
    /// Activates a spell, making it ready to be used
    /// </summary>
    public void SetActive(Entity<SpellComponent?> ent, bool active)
    {
        if (!_spellQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        ent.Comp.Active = active;
        Dirty(ent);
    }

    /// <summary>
    /// Checks whether the spell is valid,
    /// usually that means it should have its <see cref="SpellComponent.Active"/> property set to true
    /// </summary>
    public bool IsValid(Entity<SpellComponent?> ent)
    {
        if (!_spellQuery.Resolve(ent.Owner, ref ent.Comp))
            return false;

        return ent.Comp.Active;
    }

    /// <summary>
    /// Gets either all learned, or all active spells of the user
    /// </summary>
    /// <param name="ent">The spellcaster</param>
    /// <param name="activeOnly">If true, it will only return the active spells. If false, the learned ones only (non-active)</param>
    /// <returns>A list containing spells based on if they are active or not</returns>
    public List<EntityUid> GetSpells(Entity<SpellsComponent?> ent, bool activeOnly)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return new List<EntityUid>();

        var spells = new List<EntityUid>();
        foreach (var spell in ent.Comp.Container.ContainedEntities)
        {
            if (!_spellQuery.TryComp(spell, out var spellComp))
                continue;

            if (spellComp.Active != activeOnly)
                continue;

            spells.Add(spell);
        }

        return spells;
    }

    /// <summary>
    /// Checks whether we have own a specific spell
    /// </summary>
    /// <param name="ent">The spellcaster</param>
    /// <param name="spellProto">The protoype to check against</param>
    /// <returns>True if we have the spell in our container, false otherwise</returns>
    public bool HasSpell(Entity<SpellsComponent?> ent, [ForbidLiteral] EntProtoId spellProto)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return false;

        foreach (var spell in ent.Comp.Container.ContainedEntities)
        {
            if (Prototype(spell) is { } spellEntityProto && spellEntityProto == spellProto)
                return true;
        }

        return false;
    }

    #endregion

    private void LoadSpells()
    {
        AllSpells.Clear();
        var name = Factory.CompName<SpellComponent>();
        foreach (var proto in ProtoMan.EnumeratePrototypes<EntityPrototype>())
        {
            if (!proto.HasComp(name))
                return;

            var id = proto.ID;
            AllSpells.Add(id);
        }
    }
}
