using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Oathlord.Client.UserInterface.Systems.CharAttributes.Widgets;
using Content.Oathlord.Client.UserInterface.Systems.CharAttributes.Windows;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;

namespace Content.Oathlord.Client.UserInterface.Systems.CharAttributes;

[UsedImplicitly]
public sealed partial class CharacterAttributesUIController : UIController, IOnStateChanged<GameplayState>
{
    private CharacterAttributeWindow? _menu;
    private MenuButton? CharAttributeButton => UIManager.GetActiveUIWidgetOrNull<CharAttributeMenuButton>()?.CharAttributeButton;

    public override void Initialize()
    {
        base.Initialize();

        GameTopMenuBarUIController.OnLoad += OnLoad;
        GameTopMenuBarUIController.OnUnload += OnUnload;
    }

    private void OnUnload(GameTopMenuBar bar)
    {
        UnloadButton();
    }

    private void OnLoad(GameTopMenuBar bar)
    {
        LoadButton(bar);
    }

    public void OnStateEntered(GameplayState state)
    {
        //
    }

    public void OnStateExited(GameplayState state)
    {
        //
    }

    private void LoadButton(GameTopMenuBar bar)
    {
        if (CharAttributeButton == null)
            return; // todo fix

        foreach (var child in bar.Children)
        {
            if (child == CharAttributeButton)
                return;
        }

        CharAttributeButton.OnPressed += OnPressed;

        bar.AddChild(CharAttributeButton);
        CharAttributeButton.SetPositionLast();

        _menu = new CharacterAttributeWindow();
    }

    private void UnloadButton()
    {
        _menu?.Close();
        _menu = null;
        CharAttributeButton?.OnPressed -= OnPressed;
    }

    private void OnPressed(BaseButton.ButtonEventArgs obj)
    {
        ToggleCharAttributeMenu();
    }

    private void ToggleCharAttributeMenu()
    {
        if (_menu == null)
            return;

        if (_menu.IsOpen)
        {
            CharAttributeButton?.SetClickPressed(false);
            _menu.Close();
            return;
        }

        CharAttributeButton?.SetClickPressed(true);
        _menu.OpenCenteredRight();
    }
}
