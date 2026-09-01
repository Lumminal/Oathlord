using Robust.Shared.Containers;

namespace Content.Oathlord.Shared.Blacksmith.Anvil;

/// <summary>
/// Public api for <see cref="AnvilComponent"/>
///
/// todo: expand with explanations
/// </summary>
public sealed partial class AnvilSystem : EntitySystem
{
    // todo:
    // 1. Update sprite views when ent insert/remove happens

    [SubscribeLocalEvent]
    public void InsertAttempt(Entity<AnvilComponent> ent, ref ContainerIsInsertingAttemptEvent args)
    {
        if (args.Cancelled || args.Container.Count < ent.Comp.AllowedWorkables)
            return;

        args.Cancel();
    }
}
