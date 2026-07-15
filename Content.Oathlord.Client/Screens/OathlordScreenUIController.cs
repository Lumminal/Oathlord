using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Oathlord.Client.UserInterface.Systems.Mana.Widgets;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;

namespace Content.Oathlord.Client.Screens;

public sealed partial class OathlordScreenUIController : UIController
{

    public override void Initialize()
    {
        base.Initialize();

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
        gameplayStateLoad.OnScreenUnload += OnScreenUnload;
    }

    private void OnScreenUnload()
    {
        switch (UIManager.ActiveScreen)
        {
            case DefaultGameScreen screen:
                screen.RemoveWidget<ManaBar>();
                break;
            case  SeparatedChatGameScreen separated:
                separated.RemoveWidget<ManaBar>();
                break;
        }
    }

    private void OnScreenLoad()
    {
        switch (UIManager.ActiveScreen)
        {
            case DefaultGameScreen screen:
                var manaDefault = screen.GetOrAddWidget<ManaBar>();

                LayoutContainer.SetAnchorPreset(manaDefault, LayoutContainer.LayoutPreset.CenterTop, true);
                break;
            case SeparatedChatGameScreen separated:
                var manaSep = separated.GetOrAddWidget<ManaBar>();

                LayoutContainer.SetAnchorPreset(manaSep, LayoutContainer.LayoutPreset.CenterTop);
                break;
        }
    }
}
