using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Oathlord.Client.Mana;
using Content.Oathlord.Client.UserInterface.Systems.Mana.Widgets;
using Content.Oathlord.Shared.Mana;
using Content.Shared.FixedPoint;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;

namespace Content.Oathlord.Client.UserInterface.Systems.Mana;

public sealed partial class ManaUIController : UIController, IOnStateEntered<GameplayState>, IOnSystemChanged<ManaClientSystem>
{
    [Dependency] private IPlayerManager _player = default!;
    private EntityQuery<ManaUserComponent> _manaQuery = default!;

    [UISystemDependency] private readonly ManaClientSystem _mana = default!;

    public ManaBar? UI => UIManager.GetActiveUIWidgetOrNull<ManaBar>();

    public override void Initialize()
    {
        base.Initialize();

        _manaQuery = EntityManager.GetEntityQuery<ManaUserComponent>();

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

    private void SystemOnSyncMana(object? sender, (FixedPoint2 current, FixedPoint2 max, bool canUse) mana)
    {
        UI?.SyncMana(mana.current, mana.max, mana.canUse);
    }

    public void SyncMana()
    {
        if (_player.LocalEntity is not { } player)
            return;

        if (!_manaQuery.TryComp(player, out var mana))
        {
            SystemOnSyncMana(_mana, (1, 1, false));
            return;
        }

        var manaUser = (player, mana);

        var current = _mana.GetMana(manaUser);
        var max = _mana.GetMaxMana(manaUser);
        var canUse = _mana.CanUseMana(manaUser);
        SystemOnSyncMana(_mana, (current, max, canUse));
    }
}
