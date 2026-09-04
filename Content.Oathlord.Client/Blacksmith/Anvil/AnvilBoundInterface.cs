using Content.Oathlord.Shared.Blacksmith.Anvil;
using Content.Oathlord.Shared.Blacksmith.Anvil.Prototypes;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Oathlord.Client.Blacksmith.Anvil;

[UsedImplicitly]
public sealed partial class AnvilBoundInterface : BoundUserInterface
{
    private readonly ClientAnvilSystem _anvil;
    private AnvilWindow? _window;

    public AnvilBoundInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _anvil = EntMan.System<ClientAnvilSystem>();
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<AnvilWindow>();
        _window.SetOwner(Owner);
        _window.UpdateWindow();

        _anvil.UpdateWindow += UpdateWindow;

        _window.RecipeSelected += RecipeSelected;
    }

    private void UpdateWindow(object? sender, EventArgs e)
    {
        _window?.UpdateWindow();
    }

    private void RecipeSelected(ProtoId<AnvilRecipePrototype> recipe)
    {
        SendPredictedMessage(new AnvilRecipeSelectedMessage(recipe));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            _anvil.UpdateWindow -= UpdateWindow;
    }
}

