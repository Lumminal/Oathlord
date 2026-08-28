using Content.IntegrationTests.Fixtures;
using Content.IntegrationTests.Fixtures.Attributes;
using Content.Oathlord.Shared.Spellcasting.Systems;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._Oathlord;

public sealed class SpellTest : GameTest
{
    [SidedDependency(Side.Server)] private SpellcastingSystem _spellcasting = default!;

    private static readonly EntProtoId DebugSpell = "SpellDebug";

    [Test]
    [Description("Tests basic spell operations like transferring, adding, removing etc")]
    public async Task TestSpells()
    {
        // implement
    }
}
