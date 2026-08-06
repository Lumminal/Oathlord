using System.Collections.Generic;
using Content.IntegrationTests.Fixtures;
using Content.IntegrationTests.Fixtures.Attributes;
using Content.Oathlord.Shared.Economy.Components;
using Content.Oathlord.Shared.Economy.Prototypes;
using Content.Oathlord.Shared.Economy.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._Oathlord;

[TestFixture]
public sealed class EconomyTest : GameTest
{
    [SidedDependency(Side.Server)] private OathlordEconomySystem _economy = default!;

    /// <summary>
    /// The starting currencies of the economy we're gonna test against.
    /// todo: do unhardcode the prototypes lol
    /// </summary>
    public static readonly Dictionary<ProtoId<EconomyCurrencyPrototype>, int> StartingEconomy = new()
    {
        { "Na", 100 },
        { "Nar", 2},
        { "Nara", 20},
    };

    /// <summary>
    /// Tests that transactions work on a given entity with an account
    /// TODO: Add loan support sometime
    /// Tests include:
    /// - Depositing
    /// - Withdrawing
    /// </summary>
    [Test]
    public async Task TransactionsTest()
    {
        var map = await Pair.CreateTestMap();

        // Setup the economy with default currencies, and test operations against an account
        await Server.WaitAssertion(() =>
        {
            var econ = SEntMan.EnsureComponent<EconomyMapComponent>(map.MapUid);
            var ent = SEntMan.SpawnEntity(null, new MapCoordinates());
            var account = SEntMan.EnsureComponent<EconomyAccountComponent>(ent);

            Entity<EconomyAccountComponent> accEnt = (ent, account);
            Entity<EconomyMapComponent> econMap = (map.MapUid, econ);

            _economy.SetStoredCurrencies(econMap.AsNullable(), StartingEconomy);
            Server.RunTicks(5);

            // Test depositing on an account
            _economy.AddCurrencyToAccount(accEnt.AsNullable(), 10, map.MapUid);
            Assert.That(accEnt.Comp.Stored, Is.EqualTo(10));

            // Test withdrawing on an account
            _economy.WithdrawFromAccount(accEnt.AsNullable(), 2);
            Assert.That(accEnt.Comp.Stored, Is.EqualTo(8));

            // Try deposit more than economy total stored
            var total = _economy.GetTotalEconomyStored(econMap.AsNullable());
            _economy.AddCurrencyToAccount(accEnt.AsNullable(), total);
            Assert.That(accEnt.Comp.Stored, Is.EqualTo(8)); // should stay the same

            // Try to withdraw more than we have stored
            var previousStored = _economy.GetTotalEconomyStored(econMap.AsNullable());
            _economy.TryWithdrawFromEconomy(econMap.AsNullable(),
                new Dictionary<ProtoId<EconomyCurrencyPrototype>, int>
            {
                { "Nara", 10000 } // todo: do unhardcode this sometime
            });
            Assert.That(econMap.Comp.TotalStored, Is.EqualTo(previousStored)); // should stay the same
        });
    }
}
