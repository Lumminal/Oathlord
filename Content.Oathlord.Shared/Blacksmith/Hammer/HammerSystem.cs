using Content.Oathlord.Shared.Blacksmith.Anvil;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;

namespace Content.Oathlord.Shared.Blacksmith.Hammer;

public sealed partial class HammerSystem : EntitySystem
{
    [Dependency] private SharedUserInterfaceSystem _ui = default!;
    [SubscribeLocalEvent]
    public void OnDropped(Entity<HammerComponent> ent, ref DroppedEvent args)
    {
        _ui.CloseUi(args.User, AnvilUiKey.Key);
    }

    [SubscribeLocalEvent]
    public void OnBeforeAnvilOpen(Entity<HammerComponent> ent, ref HeldRelayedEvent<CanOperateAnvilAttempt> args)
    {
        // we have a hammer on our hands, so we can use the anvil
        args.Args.Handled = true;
    }
}
