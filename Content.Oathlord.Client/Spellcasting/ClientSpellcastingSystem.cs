using Content.Oathlord.Shared.Spellcasting.Components;
using Content.Oathlord.Shared.Spellcasting.Systems;
using Robust.Client.Player;

namespace Content.Oathlord.Client.Spellcasting;

public sealed partial class ClientSpellcastingSystem : SpellcastingSystem
{
    [Dependency] private IPlayerManager _player = default!;

    public EntityUid? ActiveSelectedSpell
    {
        get
        {
            var ent = _player.LocalEntity;
            return ent is { } player
                ? GetActiveSpell(player)
                : null;
        }
    }

    [SubscribeLocalEvent]
    public void OnRequestTransfer(Entity<SpellsComponent> ent, ref RequestSpellTransferEvent args)
    {
        var ev = new SpellTransferEvent(GetNetEntity(args.Spell), args.Type);
        RaisePredictiveEvent(ev);
        if (!ev.Cancelled)
            return;

        args.Cancelled = true;
    }

    public void RequestActiveSpell()
    {

    }
}
