using Content.Oathlord.Shared.Mana;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Oathlord.Server.Mana.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed partial class AdjustManaCommand : LocalizedEntityCommands
{
    [Dependency] private ManaSystem _mana = default!;

    public override string Command => "adjustmana";
    public override string Description => "Adjusts the current mana of the entity";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("cmd-addaction-invalid-args"));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var targetUidNet)
            || !EntityManager.TryGetEntity(targetUidNet, out var targetEntity))
        {
            shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
            return;
        }

        if (!EntityManager.HasComponent<ManaUserComponent>(targetEntity))
        {
            shell.WriteError("Entity doesn't have mana user component");
            return;
        }

        if (!float.TryParse(args[1], out var amount))
        {
            shell.WriteError("Argument must be a number");
            return;
        }

        if (targetEntity is not { } target)
        {
            shell.WriteError("Target is null");
            return;
        }

        _mana.AdjustMana(target, amount);
    }
}
