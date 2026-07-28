using Content.Oathlord.Shared.Economy.Components;
using Content.Oathlord.Shared.Economy.Prototypes;
using Content.Oathlord.Shared.Economy.Systems;
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

       _window.RequestTransaction += RequestTransaction;
    }

    private void RequestTransaction(NetEntity? entity, Dictionary<ProtoId<EconomyCurrencyPrototype>, int> currencies, EconomyTransaction type)
    {
        SendPredictedMessage(new EconomyTransactionMessage(entity, currencies, type));
        _window?.UpdateInfo();
    }
}
