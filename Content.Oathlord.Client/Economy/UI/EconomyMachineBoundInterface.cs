using Robust.Client.UserInterface;

namespace Content.Oathlord.Client.Economy.UI;

public sealed partial class EconomyMachineBoundInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private EconomyMachineWindow? _window;

    protected override void Open()
    {
        base.Open();

       _window = this.CreateWindow<EconomyMachineWindow>();
       _window.SetOwner(Owner);

       _window.Populate();
    }
}
