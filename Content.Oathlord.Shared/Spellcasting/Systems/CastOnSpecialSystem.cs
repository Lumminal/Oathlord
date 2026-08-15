using Content.Oathlord.Shared.ItemSpecial;
using Content.Oathlord.Shared.Spellcasting.Components;

namespace Content.Oathlord.Shared.Spellcasting.Systems;

public sealed class CastOnSpecialSystem : EntitySystem
{
    [SubscribeLocalEvent]
    public void OnItemSpecial(Entity<CastOnSpecialComponent> ent, ref ItemSpecialEvent args)
    {
        // implement
    }
}
