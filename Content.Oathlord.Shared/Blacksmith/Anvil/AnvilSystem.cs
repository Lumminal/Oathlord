using System.Linq;
using Content.Oathlord.Shared.Blacksmith.Anvil.Prototypes;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Blacksmith.Anvil;

/// <summary>
/// Public api for <see cref="AnvilComponent"/>
///
/// todo: expand with explanations
/// </summary>
public sealed partial class AnvilSystem : EntitySystem
{
    // todo:
    // 1. Update sprite views when ent insert/remove happens
    [Dependency] private EntityQuery<MetalWorkableComponent> _metalQuery = default!;

    /// <summary>
    /// A dictionary of all metals matched with their respective recipes, for fast lookups
    /// </summary>
    [ViewVariables]
    public List<ProtoId<AnvilRecipePrototype>> Recipes = new();

    public override void Initialize()
    {
        base.Initialize();

        LoadMetalRecipes();
    }

    [SubscribeLocalEvent]
    public void OnProtoReload(PrototypesReloadedEventArgs args)
    {
        if (!args.WasModified<AnvilRecipePrototype>())
            return;

        LoadMetalRecipes();
    }

    [SubscribeLocalEvent]
    public void InsertAttempt(Entity<AnvilComponent> ent, ref ContainerIsInsertingAttemptEvent args)
    {
        if (args.Cancelled || args.Container.Count < ent.Comp.AllowedWorkables)
            return;

        args.Cancel();
    }

    /// <summary>
    /// Gets all recipes, given a list of workable metal entities.
    /// </summary>
    /// <param name="metals">The metals we want to get recipes from</param>
    /// <returns>A list of all recipes matching the metals</returns>
    /// <remarks>
    /// This is done this way because we want to support complex recipes.
    /// The other way would be to store recipes in metals, but this makes it harder to do complex recipes
    /// E.g. iron ingot, iron ingot, gold ingot.
    ///
    /// Doing it with <see cref="AnvilRecipePrototype"/> makes the process easier, along with adding more recipes easier.
    /// However, it has high complexity, although that won't be a problem for roughly 100 recipes (if we get to that point).
    ///
    /// We ❤️ LINQ
    /// </remarks>
    public List<ProtoId<AnvilRecipePrototype>> GetRecipes(List<EntityUid> metals)
    {
        var recipes = new List<ProtoId<AnvilRecipePrototype>>();
        if (metals.Count == 0)
            return recipes;

        var metalProtos = new List<EntProtoId>();
        foreach (var metal in metals)
        {
            if (!_metalQuery.HasComp(metal))
            {
                // this shouldn't happen unless programmer error
                Log.Error($"During fetching metal recipes, it tried to check against a non-workable metal entity: ${ToPrettyString(metal)}");
                return recipes;
            }

            var proto = Prototype(metal);
            if (proto is not { } prototype)
                continue;

            metalProtos.Add(prototype);
        }

        var metalProtoSorted = metalProtos.OrderBy(m => m.Id).ToList();
        foreach (var recipe in Recipes)
        {
            if (!ProtoMan.Resolve(recipe, out var recipeProto))
                continue;

            var metalRecipes = recipeProto.Metals;
            if (metalRecipes.Count != metalProtos.Count)
                continue;

            var metalRecipeSorted =  metalProtos.OrderBy(m => m.Id).ToList();
            if (!metalRecipeSorted.SequenceEqual(metalProtoSorted))
                continue;

            recipes.Add(recipeProto);
        }

        return recipes;
    }

    private void LoadMetalRecipes()
    {
        Recipes.Clear();
        foreach (var recipe in ProtoMan.EnumeratePrototypes<AnvilRecipePrototype>())
        {
            Recipes.Add(recipe);
        }
    }
}
