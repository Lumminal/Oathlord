using Content.Oathlord.Shared.Spellcasting.Components;
using Content.Oathlord.Shared.Spellcasting.Systems;
using Robust.Client.Player;

namespace Content.Oathlord.Client.Spellcasting;

public sealed partial class ClientSpellcastingSystem : SpellcastingSystem
{
    [Dependency] private IPlayerManager _player = default!;

    public event EventHandler<int>? SelectedSpellChanged;
    public event EventHandler? UpdateSpellWindow;

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

    protected override void UpdateUi(Entity<SpellsComponent> ent, bool refresh = false)
    {
        base.UpdateUi(ent, refresh);

        if (_player.LocalEntity != ent.Owner)
            return;

        SelectedSpellChanged?.Invoke(this, ent.Comp.SelectedSpell);

        if (refresh)
            UpdateSpellWindow?.Invoke(this, EventArgs.Empty);
    }
}
