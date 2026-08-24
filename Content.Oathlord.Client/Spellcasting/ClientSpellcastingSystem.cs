using Content.Oathlord.Shared.Spellcasting.Components;
using Content.Oathlord.Shared.Spellcasting.Systems;

namespace Content.Oathlord.Client.Spellcasting;

public sealed partial class ClientSpellcastingSystem : SpellcastingSystem
{
    [SubscribeLocalEvent]
    public void OnRequestTransfer(Entity<SpellsComponent> ent, ref RequestSpellTransferEvent args)
    {
        // RaisePredictiveEvent does not support passing events by reference, so we must pass EntityEventArgs lesgooo
        var ev = new SpellTransferEvent(GetNetEntity(args.Spell), args.Type);
        RaisePredictiveEvent(ev);
    }
}
