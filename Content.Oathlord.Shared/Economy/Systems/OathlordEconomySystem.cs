using Content.Oathlord.Shared.Economy.Components;
using Content.Shared.GameTicking;

namespace Content.Oathlord.Shared.Economy.Systems;

/// <summary>
///
/// </summary>
public sealed partial class OathlordEconomySystem : EntitySystem
{
    /// <summary>
    /// The entity in which the current active economy is stored, for quick access
    /// Only one economy can be active during a round
    /// </summary>
    [ViewVariables]
    public Entity<EconomyMapComponent>? Economy;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EconomyAccountComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<EconomyMapComponent, MapInitEvent>(OnEconomyInit);

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnReset);
    }

    private void OnMapInit(Entity<EconomyAccountComponent> ent, ref MapInitEvent args)
    {
        AddAccountToEconomy(ent);
    }

    private void OnEconomyInit(Entity<EconomyMapComponent> ent, ref MapInitEvent args)
    {
        if (Economy != null)
        {
            Log.Error("Tried to initialize more than one economy. You may only have 1 at any time");
            return;
        }

        Economy = ent;
        Log.Info("Initialized economy");
    }

    private void OnReset(RoundRestartCleanupEvent args)
    {
        Economy = null;
        Log.Info("Uninitialized economy");
    }

    /// <summary>
    /// Adds an account to the economy
    /// </summary>
    public void AddAccountToEconomy(Entity<EconomyAccountComponent> account)
    {
        if (Economy is not { } economy)
            return;

        economy.Comp.ActiveAccounts.Add(account);
        Dirty(economy);

        Log.Info("Added account to economy");
    }

}
