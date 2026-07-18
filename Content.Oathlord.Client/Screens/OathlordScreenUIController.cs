using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Oathlord.Client.UserInterface.Systems.Mana;
using Content.Oathlord.Client.UserInterface.Systems.Mana.Widgets;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;

namespace Content.Oathlord.Client.Screens;

/// <summary>
/// If you need to add an element to the game's HUD, you came to the right place
/// For future reference, adding a UI element goes like this:
/// - Make a UI Widget
/// - Make a UI controller to handle updating your widget via GetActiveWidgetOrNull (this gets the active screen's widget)
/// - Add it here via the methods relating to widgets (RemoveWidget, GetOrAddWidget etc)
/// and don't forget to account for both separated (ss13) + default (ss14) screens
///
/// ps: I'd prefer doing an all gwyn ds1 run than working with ss14 ui
/// </summary>
public sealed partial class OathlordScreenUIController : UIController
{
    private ManaBar? _manaBar;
    private ManaUIController? _manaUIController;

    public override void Initialize()
    {
        base.Initialize();

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
        gameplayStateLoad.OnScreenUnload += OnScreenUnload;

        _manaUIController = UIManager.GetUIController<ManaUIController>();
    }

    private void OnScreenUnload()
    {
        _manaBar = null;

        switch (UIManager.ActiveScreen)
        {
            case DefaultGameScreen screen:
                screen.RemoveWidget<ManaBar>();
                break;
            case SeparatedChatGameScreen separated:
                separated.RemoveWidget<ManaBar>();
                break;
        }
    }

    private void OnScreenLoad()
    {
        switch (UIManager.ActiveScreen)
        {
            case DefaultGameScreen screen:
                _manaBar = screen.GetOrAddWidget<ManaBar>();
                _manaUIController?.SyncMana(); // forcing a sync here cuz it doesn't sync otherwise

                LayoutContainer.SetAnchorAndMarginPreset(_manaBar, LayoutContainer.LayoutPreset.BottomWide, margin: 40);
                break;
            case SeparatedChatGameScreen separated:
                _manaBar = separated.GetOrAddWidget<ManaBar>();
                _manaUIController?.SyncMana(); // same as above

                LayoutContainer.SetAnchorAndMarginPreset(_manaBar, LayoutContainer.LayoutPreset.BottomWide, margin: 40);
                LayoutContainer.SetMarginLeft(_manaBar, -460);
                break;
        }
    }
}
