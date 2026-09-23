using Content.Client.Lobby;
using Content.Client.Lobby.UI;
using Content.Client.UserInterface.Systems.Character.Windows;
using Content.Oathlord.Client.Oaths.UI;
using Content.Oathlord.Client.Oaths.UI.Controls;
using Content.Oathlord.Shared.Oaths;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controls;

namespace Content.Oathlord.Client.Oaths;

public sealed partial class ClientOathSystem : OathSystem
{
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();

        CharacterWindow.OnOpened += EnsureOath;
        LobbyUIController.OnProfileEditorCreated += OnProfileEditor;
    }

    public override void Shutdown()
    {
        base.Shutdown();

        CharacterWindow.OnOpened -= EnsureOath;
        LobbyUIController.OnProfileEditorCreated -= OnProfileEditor;
    }

    private void OnProfileEditor(HumanoidProfileEditor editor)
    {
        var above = editor.TraitsTab;
        var index = above.GetPositionInParent();

        var tab = new OathProfileEditor(ProtoMan, this, _sprite);
        tab.OnSave += oath =>
        {
            editor.Profile = editor.Profile?.WithOath(oath);
            editor.IsDirty = true;
        };

        editor.TabContainer.AddChild(tab);
        tab.SetPositionInParent(index);
        TabContainer.SetTabTitle(tab, "Oaths");
    }

    private void EnsureOath(CharacterWindow window)
    {
        if (_player.LocalEntity is not { } player|| GetOath(player) is not { } oath)
            return;

        // todo: test
        var oathControl = new OathControl(oath, ProtoMan, _sprite);
        oathControl.MaxSize = new Vector2(64, 64);

        window.CharacterContainer.AddChild(oathControl);
    }
}
