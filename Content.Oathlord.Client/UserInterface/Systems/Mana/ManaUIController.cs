using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Oathlord.Client.Mana;
using Content.Oathlord.Client.UserInterface.Systems.Mana.Widgets;
using Content.Shared.FixedPoint;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;

namespace Content.Oathlord.Client.UserInterface.Systems.Mana;

public sealed partial class ManaUIController : UIController, IOnStateEntered<GameplayState>, IOnSystemChanged<ManaClientSystem>
{
    [Dependency] private IPlayerManager _player = default!;
    [UISystemDependency] private readonly ManaClientSystem _mana = default!;

    private ManaBar? UI => UIManager.GetActiveUIWidgetOrNull<ManaBar>();

    public override void Initialize()
    {
        base.Initialize();

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
    }

    private void OnScreenLoad()
    {
        SyncMana();
    }

    public void OnSystemLoaded(ManaClientSystem system)
    {
        system.SyncMana += SystemOnSyncMana;
    }

    public void OnSystemUnloaded(ManaClientSystem system)
    {
        system.SyncMana -= SystemOnSyncMana;
    }

    public void OnStateEntered(GameplayState state)
    {
        SyncMana();
    }

    private void SystemOnSyncMana(object? sender, FixedPoint2 e)
    {
        UI?.SyncMana(e);
    }

    public void SyncMana()
    {
        if (_player.LocalEntity is not { } player)
            return;

        SystemOnSyncMana(_mana, _mana.GetMana(player));
    }
}
