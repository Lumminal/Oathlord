using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Oathlord.Client.UserInterface.Systems.Spells.Controls;
using Content.Oathlord.Client.UserInterface.Systems.Spells.Widgets;
using Content.Oathlord.Client.UserInterface.Systems.Spells.Windows;
using Content.Oathlord.Common.Input;
using Content.Oathlord.Shared.Spellcasting.Components;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;

namespace Content.Oathlord.Client.UserInterface.Systems.Spells;

[UsedImplicitly]
public sealed partial class SpellsUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
    [Dependency] private IPlayerManager _player = default!;

    private EntityQuery<SpellsComponent> _spellsQuery = default!;

    private SpellsWindow? _window;
    private SpellsButton? UI => UIManager.GetActiveUIWidgetOrNull<SpellsButton>();

    public void OnStateEntered(GameplayState state)
    {
        _window = UIManager.CreateWindow<SpellsWindow>();
        LayoutContainer.SetAnchorPreset(_window, LayoutContainer.LayoutPreset.Center);

        var builder = CommandBinds.Builder;
        builder
            .Bind(OathlordKeyFunctions.OpenSpellsMenu,
                InputCmdHandler.FromDelegate(_ => ToggleWindow()))
            .Register<SpellsUIController>();
    }

    public void OnStateExited(GameplayState state)
    {
        if (_window != null)
        {
            _window.Close();
            _window = null;
        }

        CommandBinds.Unregister<SpellsUIController>();
    }

    public override void Initialize()
    {
        base.Initialize();

        _spellsQuery = EntityManager.GetEntityQuery<SpellsComponent>();

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += LoadButton;
        gameplayStateLoad.OnScreenUnload += UnloadButton;
    }

    public void LoadButton()
    {
        if (UI == null)
            return;

        UI.OpenSpellsButton.OnPressed += SpellsButtonOnOnPressed;
    }

    public void UnloadButton()
    {
        if (UI == null)
            return;

        UI.OpenSpellsButton.OnPressed -= SpellsButtonOnOnPressed;
    }

    private void SpellsButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
    {
        ToggleWindow();
    }

    public void UpdateWindow()
    {
        // todo: this is temporarily until I add client systems to update stuff
        if (_window == null || _player.LocalEntity is not { } player || !_spellsQuery.TryComp(player, out var spells))
            return;

        var maxLearned = spells.MaxLearned;
        var currentSlots = spells.CurrentSlots;

        var learnedSpellContainer = _window.LearnedSpellsContainer;
        var activeSpellContainer = _window.ActiveSpellsContainer;

        learnedSpellContainer.Children.Clear();
        activeSpellContainer.Children.Clear();

        // Setup how many learned spells we can have at a time
        for (int i = 0; i < maxLearned; i++)
        {
            var spell = new SpellSlot();
            learnedSpellContainer.AddChild(spell);
        }

        // Setup how many active spell slots we can have
        for (int i = 0; i < currentSlots; i++)
        {
            var spell = new SpellSlot();
            activeSpellContainer.AddChild(spell);
        }

        foreach (var learnedSpell in spells.LearnedSpells)
        {
            foreach (var learnedSpellSlot in learnedSpellContainer.Children)
            {
                if (learnedSpellSlot is not SpellSlot spellSlot || spellSlot.HasSpell)
                    continue;

                spellSlot.AddSpell(learnedSpell);
                break;
            }
        }
    }

    private void ToggleWindow()
    {
        if (_window == null)
            return;

        UI?.OpenSpellsButton.SetClickPressed(!_window.IsOpen);

        if (_window.IsOpen)
        {
            _window.Close();
            return;
        }

        _window = UIManager.CreateWindow<SpellsWindow>();
        UpdateWindow();

        _window.Open();
    }
}
