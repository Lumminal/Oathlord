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
        if (UIManager.ActiveScreen is DefaultGameScreen screen)
        {
            screen.RemoveWidget<ManaBar>();
        }
    }

    private void OnScreenLoad()
    {
        // get widgetn
        switch (UIManager.ActiveScreen)
        {
            case DefaultGameScreen screen:
                break;
            case SeparatedChatGameScreen separated:
                break;
        }

        //LayoutContainer.SetAnchorAndMarginPreset(mana, LayoutContainer.LayoutPreset.TopWide, margin: 100);
    }


}
