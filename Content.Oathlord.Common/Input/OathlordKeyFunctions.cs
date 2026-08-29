using Robust.Shared.Input;

namespace Content.Oathlord.Common.Input;

[KeyFunctions]
public static class OathlordKeyFunctions
{
    // Activation
    public static readonly BoundKeyFunction SpecialItemAction = "SpecialItemAction";

    // Spellcasting
    public static readonly BoundKeyFunction OpenSpellsMenu = "OpenSpellsMenu";
    public static readonly BoundKeyFunction MoveSpellUp = "MoveSpellUp";
    public static readonly BoundKeyFunction MoveSpellDown = "MoveSpellDown";
}
