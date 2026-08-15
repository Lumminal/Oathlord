using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Oathlord.Client.UserInterface.Systems.Spells.Widgets;
using Content.Oathlord.Client.UserInterface.Systems.Spells.Windows;
using Content.Oathlord.Common.Input;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;

namespace Content.Oathlord.Client.UserInterface.Systems.Spells;

[UsedImplicitly]
public sealed partial class SpellsUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
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

        _window.Open();
    }
}
