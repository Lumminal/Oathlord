using Content.Shared.Verbs;

namespace Content.Oathlord.Shared.Oaths;

public sealed partial class OathProviderSystem : EntitySystem
{
    [Dependency] private OathSystem _oath = default!;

    [SubscribeLocalEvent]
    public void OnVerb(Entity<OathProviderComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        var user = args.User;
        if (!args.CanInteract || !args.CanInteract || _oath.GetOath(user) == null)
            return;

        args.Verbs.Add(new AlternativeVerb
        {
            // todo need an icon for this
            Text = "Pray to Avatar",
            Act = () => _oath.SetOath(user, ent.Comp.Oath)
        });
    }
}
