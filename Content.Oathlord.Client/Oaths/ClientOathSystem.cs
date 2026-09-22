using Content.Client.Lobby;
using Content.Client.Lobby.UI;
using Content.Oathlord.Client.Oaths.UI;
using Content.Oathlord.Shared.Oaths;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;

namespace Content.Oathlord.Client.Oaths;

public sealed partial class ClientOathSystem : OathSystem
{
    [Dependency] private SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();

        LobbyUIController.OnProfileEditorCreated += OnProfileEditor;
    }

    public override void Shutdown()
    {
        base.Shutdown();

        LobbyUIController.OnProfileEditorCreated -= OnProfileEditor;
    }

    private void OnProfileEditor(HumanoidProfileEditor editor)
    {
        var above = editor.MarkingsTab;
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
}
