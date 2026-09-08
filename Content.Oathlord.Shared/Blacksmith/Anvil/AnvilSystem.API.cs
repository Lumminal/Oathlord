using System.Linq;
using Content.Oathlord.Shared.Blacksmith.Anvil.Prototypes;
using Content.Shared.Storage;
using Robust.Shared.Prototypes;

namespace Content.Oathlord.Shared.Blacksmith.Anvil;

public partial class AnvilSystem
{
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
                Log.Error($"During fetching metal recipes, tried to check against a non-workable metal entity prototype: ${ToPrettyString(metal)}");
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

    /// <summary>
    /// Sets the selected recipe of the anvil
    /// </summary>
    /// <param name="ent">The anvil</param>
    /// <param name="recipe">The recipe to select</param>
    public void SetSelectedRecipe(Entity<AnvilComponent?> ent, [ForbidLiteral] ProtoId<AnvilRecipePrototype> recipe)
    {
        if (!_anvilQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        ent.Comp.SelectedRecipe = recipe;
        DirtyField(ent, nameof(AnvilComponent.SelectedRecipe));
    }

    /// <summary>
    /// Adjusts the work that was done on the current recipe
    /// </summary>
    /// <param name="ent">The anvil</param>
    /// <param name="number">The work to adjust</param>
    public void AdjustWorkDone(Entity<AnvilComponent?> ent, int number)
    {
        if (!_anvilQuery.Resolve(ent.Owner, ref ent.Comp))
            return;

        // todo for future expansion:
        // once we get cooling/heating metals, raise event here for every metal so it checks against the current temp

        ent.Comp.WorkDone = Math.Clamp(ent.Comp.WorkDone + number, 0, 100); // todo: should not be 100 max once we get more complex recipes (maybe add recipe categories for higher?)
        DirtyField(ent, nameof(AnvilComponent.WorkDone));
    }

    #region Helpers

    /// <summary>
    /// Tries to clean and delete the container containing the metals
    /// </summary>
    /// <param name="ent">The anvil</param>
    /// <returns>True if the container could be cleaned, false otherwise</returns>
    private bool TryCleanMetals(Entity<AnvilComponent> ent)
    {
        if (!_container.TryGetContainer(ent, StorageComponent.ContainerId, out var container))
        {
            Log.Error($"Could not find anvil's ({ToPrettyString(ent)}) storage container");
            return false;
        }

        _container.CleanContainer(container);
        return true;
    }

    /// <summary>
    /// Resets the recipe on the anvil, as well as the work done on the previous recipe (if any was done)
    /// </summary>
    private void ResetAnvil(Entity<AnvilComponent> ent, string containerId)
    {
        if (containerId != StorageComponent.ContainerId)
            return;

        if (!_timing.ApplyingState)
        {
            ent.Comp.SelectedRecipe = null;
            ent.Comp.WorkDone = 0;
            Dirty(ent);
        }

        // container mispredict hellbugs
        UpdateUi();
    }

    private void LoadMetalRecipes()
    {
        Recipes.Clear();
        foreach (var recipe in ProtoMan.EnumeratePrototypes<AnvilRecipePrototype>())
        {
            Recipes.Add(recipe);
        }
    }

    #endregion

    /// <summary>
    /// Refreshes the anvil window
    /// </summary>
    protected virtual void UpdateUi() { }
}
