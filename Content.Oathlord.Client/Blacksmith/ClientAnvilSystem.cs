using Content.Oathlord.Shared.Blacksmith.Anvil;

namespace Content.Oathlord.Client.Blacksmith;

public sealed partial class ClientAnvilSystem : AnvilSystem
{
    /// <summary>
    /// Updates the window
    /// </summary>
    public event EventHandler? UpdateWindow;

    [SubscribeLocalEvent]
    public void OnAutoHandleState(Entity<AnvilComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        // todo: figure out why container is not working, solution rework into item slots and be done with it or something
        UpdateWindow?.Invoke(this, EventArgs.Empty);
    }
}
