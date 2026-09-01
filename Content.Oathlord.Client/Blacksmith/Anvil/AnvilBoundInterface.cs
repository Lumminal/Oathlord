using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Oathlord.Client.Blacksmith.Anvil;

[UsedImplicitly]
public sealed class AnvilBoundInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private AnvilWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<AnvilWindow>();

        _window.SetOwner(Owner);
        _window.UpdateWindow();

        _window.OpenCentered();
    }
}

