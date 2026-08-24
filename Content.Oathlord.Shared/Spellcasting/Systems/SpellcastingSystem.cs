using Content.Oathlord.Shared.Spellcasting.Components;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Spellcasting.Systems;

/// <summary>
/// Handles anything related to spellcasting, and provides a public api
/// </summary>
public abstract partial class SpellcastingSystem : EntitySystem
{
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

        SubscribeAllEvent<SpellTransferEvent>(OnTransfer);

        LoadSpells();
    }

    [SubscribeLocalEvent]
    public void OnCompInit(Entity<SpellsComponent> ent, ref ComponentInit args)
    {
        ent.Comp.Container = _container.EnsureContainer<Container>(ent, SpellsComponent.ContainerId);
    }

    [SubscribeLocalEvent]
    public void OnMapInit(Entity<SpellsComponent> ent, ref MapInitEvent args)
    {
        // debug shit
        var debug = Spawn("SpellDebug");
        var debug2 = Spawn("SpellDebug");

        InsertSpell(ent.AsNullable(), debug);
        InsertSpell(ent.AsNullable(), debug2);

        DirtyField(ent.AsNullable(), nameof(SpellsComponent.LearnedSpells));
    }

    public void OnTransfer(SpellTransferEvent msg, EntitySessionEventArgs args)
    {
        var attached = args.SenderSession.AttachedEntity;
        if (attached is not { } entity)
            return;

        var spell = GetEntity(msg.Spell);
        var transferTo = msg.Type;

        if (TryTransferSpell(entity, spell, transferTo))
            return;

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

        switch (spellTransferType)
        {
            case SpellTransfer.Active:
            {
                if (ent.Comp.Slots.Contains(spell) || ent.Comp.Slots.Count >= ent.Comp.CurrentSlots)
                    return false;
                break;
            }
            case SpellTransfer.Learned:
            {
                if (ent.Comp.LearnedSpells.Contains(spell) || ent.Comp.LearnedSpells.Count >= ent.Comp.MaxLearned)
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
                ent.Comp.LearnedSpells.Remove(spell);
                ent.Comp.Slots.Add(spell);
                break;
            }
            case SpellTransfer.Learned:
            {
                ent.Comp.Slots.Remove(spell);
                ent.Comp.LearnedSpells.Add(spell);
                break;
            }
        }

        DirtyFields(ent, null, nameof(SpellsComponent.Slots), nameof(SpellsComponent.LearnedSpells));

        // UpdateUi here...
    }

    /// <summary>
    /// Casts the active spell
    /// </summary>
    public void CastSpell(Entity<SpellsComponent?> ent, EntityUid target, EntityCoordinates coords)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        var spell = GetActiveSpell(ent);
        if (_actions.GetAction(spell) is not { } action)
            return;

        // This function is usually called by item special (because that's how you cast spells)
        // Item special has a PointerInputCmdHandler, which returns an entity.
        // Although that entity is not null, it can be invalid (meaning its id is 0).
        // todo: make it nullable instead?
        if (target.Valid)
            _actions.SetEventTarget(action, target);

        // for world actions, we have to set the coordinates manually
        if (_worldActionQuery.TryComp(action, out var worldAction) && worldAction.Event is { } worldEv)
        {
            worldEv.Target = coords;
            worldEv.Entity = target.Valid ? target : null;
        }

        _actions.PerformAction(ent.Owner, action);
    }

    /// <summary>
    /// Gets the active spell
    /// </summary>
    public EntityUid? GetActiveSpell(Entity<SpellsComponent?> ent)
    {
        if (!_spellsQuery.Resolve(ent.Owner, ref ent.Comp))
            return null;

        if (ent.Comp.ActiveSpell >= ent.Comp.Slots.Count || ent.Comp.ActiveSpell < 0)
            return null;

        return ent.Comp.Slots[ent.Comp.ActiveSpell];
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

        Log.Info("Spell inserted into the container");

        ent.Comp.LearnedSpells.Add(spell);
        DirtyField(ent, nameof(SpellsComponent.LearnedSpells));
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
