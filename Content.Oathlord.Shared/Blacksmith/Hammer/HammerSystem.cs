using Content.Oathlord.Shared.Blacksmith.Anvil;
using Content.Shared.Hands;

namespace Content.Oathlord.Shared.Blacksmith.Hammer;

public sealed partial class HammerSystem : EntitySystem
{
    [SubscribeLocalEvent]
    public void OnBeforeAnvilOpen(Entity<HammerComponent> ent, ref HeldRelayedEvent<CanOperateAnvilAttempt> args)
    {
        // we have a hammer on our hands, so we can use the anvil
        args.Args.Handled = true;
    }
}
