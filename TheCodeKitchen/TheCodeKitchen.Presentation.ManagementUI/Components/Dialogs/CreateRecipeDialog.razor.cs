using Microsoft.AspNetCore.Components;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Models.Recipe;
using TheCodeKitchen.Application.Contracts.Requests.CookBook;
using TheCodeKitchen.Application.Contracts.Response.CookBook;
using TheCodeKitchen.Application.Contracts.Response.Pantry;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;
using TheCodeKitchen.Presentation.ManagementUI.Validation;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Dialogs;

public partial class CreateRecipeDialog(
    ISnackbar snackbar,
    IClusterClient clusterClient,
    CreateRecipeFormModelValidator validator
) : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public ICollection<GetIngredientResponse>? Ingredients { get; set; }
    [Parameter] public ICollection<GetRecipeResponse>? Recipes { get; set; }
    private string? ErrorMessage { get; set; }
    private ICollection<string> AllIngredients { get; set; } = new List<string>();
    private MudForm Form { get; set; } = new();
    private CreateRecipeFormModel Model { get; set; } = new();
    private bool Creating { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Recipes is null)
        {
            try
            {
                var cookBookGrain = clusterClient.GetGrain<ICookBookGrain>(Guid.Empty);

                var getRecipesResult = await cookBookGrain.GetRecipes();
                if (getRecipesResult.Succeeded)
                    Recipes = getRecipesResult.Value.ToList();
                else
                    ErrorMessage = getRecipesResult.Error.Message;
            }
            catch
            {
                ErrorMessage = "An error occurred while retrieving the recipe.";
            }
        }

        if (Ingredients is null)
        {
            try
            {
                var pantryGrain = clusterClient.GetGrain<IPantryGrain>(Guid.Empty);

                var getIngredientsResult = await pantryGrain.GetIngredients();
                if (getIngredientsResult.Succeeded)
                    Ingredients = getIngredientsResult.Value.ToList();
                else
                    ErrorMessage = getIngredientsResult.Error.Message;
            }
            catch
            {
                ErrorMessage = "An error occurred while retrieving the ingredients.";
            }
        }

        if (Recipes is not null && Ingredients is not null)
        {
            var recipeNames = Recipes.Select(r => r.Name).ToList();
            var ingredientNames = Ingredients.Select(i => i.Name).ToList();
            AllIngredients = recipeNames.Concat(ingredientNames).ToList();
        }

        await base.OnInitializedAsync();
    }

    private void AddStep(ICollection<StepFormModel> steps)
    {
        steps.Add(new StepFormModel());
    }

    private void RemoveStep(ICollection<StepFormModel> steps, StepFormModel step)
    {
        steps.Remove(step);
    }

    private void AddIngredient()
    {
        Model.Ingredients.Add(new IngredientFormModel());
    }

    private void RemoveIngredient(IngredientFormModel ingredient)
    {
        Model.Ingredients.Remove(ingredient);
    }

    private async Task OnIngredientNameChanged(IngredientFormModel ingredient, string value)
    {
        ingredient.Name = value;

        if (Recipes?.Select(r => r.Name).Contains(value) == true)
        {
            ingredient.Steps.Clear();
        }

        await InvokeAsync(StateHasChanged);
    }


    private async Task Submit()
    {
        await Form.Validate();
        if (!Form.IsValid)
            return;

        Creating = true;
        try
        {
            var steps = Model.Steps
                .Select(s => new RecipeStepDto(s.EquipmentType, s.Time!.Value))
                .ToList();

            var ingredients = Model.Ingredients
                .Select(i =>
                    new RecipeIngredientDto(
                        i.Name,
                        i.Steps
                            .Select(s => new RecipeStepDto(s.EquipmentType, s.Time!.Value))
                            .ToList()
                    )
                )
                .ToList();

            var request = new CreateRecipeRequest(Model.Name, steps, ingredients);
            var cookBookGrain = clusterClient.GetGrain<ICookBookGrain>(Guid.Empty);
            var createRecipeResult = await cookBookGrain.CreateRecipe(request);

            if (createRecipeResult.Succeeded)
            {
                snackbar.Add("Successfully created recipe.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(createRecipeResult.Value));
            }
            else
                snackbar.Add(createRecipeResult.Error.Message, Severity.Error);
        }
        catch
        {
            snackbar.Add("An error occured while trying to create a recipe.", Severity.Error);
        }
        finally
        {
            Creating = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}