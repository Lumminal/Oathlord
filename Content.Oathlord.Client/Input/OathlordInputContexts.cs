using Content.Oathlord.Common.Input;
using Robust.Shared.Input;

namespace Content.Oathlord.Client.Input;

public static class OathlordInputContexts
{
    public static void SetupContexts(IInputContextContainer contexts)
    {
        var common = contexts.GetContext("common");
        common.AddFunction(OathlordKeyFunctions.OpenSpellsMenu);

        var human = contexts.GetContext("human");
        human.AddFunction(OathlordKeyFunctions.SpecialItemAction);
    }
}
