using Content.Oathlord.Shared.Economy.Components;
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

       _window.RequestDeposit += RequestDeposit;
    }

    private void RequestDeposit(NetEntity? entity, int amount)
    {
        SendPredictedMessage(new EconomyDepositMessage(entity, amount));
        _window?.UpdateInfo();
    }
}
