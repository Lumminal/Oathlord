using Content.Shared.Administration;
using Robust.Client.Graphics;
using Robust.Shared.Console;

namespace Content.Oathlord.Client.Hitbox;

[AnyCommand]
public sealed partial class HitboxOverlayCommand : IConsoleCommand
{
    [Dependency] private IOverlayManager _overlay = default!;
    [Dependency] private IEntityManager _entMan = default!;

    public string Command => "toggleweaponhithox";
    public string Description => "Shows the weapon hitboxes";
    public string Help => "toggleweaponhithox";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var existing = _overlay.RemoveOverlay<HitboxWeaponOverlay>();
        if (!existing)
            _overlay.AddOverlay(new HitboxWeaponOverlay(_entMan));

        shell.WriteLine("Toggled hitbox weapon overlay...");
    }
}
