using Microsoft.AspNetCore.Components;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Response.CookBook;
using TheCodeKitchen.Application.Contracts.Response.Pantry;
using TheCodeKitchen.Presentation.ManagementUI.Components.Dialogs;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Pages;

public partial class Recipes(
    NavigationManager navigationManager,
    IDialogService dialogService,
    IClusterClient clusterClient
) : ComponentBase
{
    private ICollection<GetRecipeResponse>? GetRecipeResponses { get; set; }
    private ICollection<GetIngredientResponse>? GetIngredientResponses { get; set; }
    private string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadRecipes();
        await LoadIngredients();
        await base.OnInitializedAsync();
    }

    private async Task LoadRecipes()
    {
        try
        {
            GetRecipeResponses = null;
            var cookBookGrain = clusterClient.GetGrain<ICookBookGrain>(Guid.Empty);

            var getRecipesResult = await cookBookGrain.GetRecipes();
            if (getRecipesResult.Succeeded)
                GetRecipeResponses = getRecipesResult.Value.ToList();
            else
                ErrorMessage = getRecipesResult.Error.Message;
        }
        catch
        {
            ErrorMessage = "An error occurred while retrieving the recipe.";
        }
    }

    private async Task LoadIngredients()
    {
        try
        {
            GetIngredientResponses = null;
            var pantryGrain = clusterClient.GetGrain<IPantryGrain>(Guid.Empty);

            var getIngredientsResult = await pantryGrain.GetIngredients();
            if (getIngredientsResult.Succeeded)
                GetIngredientResponses = getIngredientsResult.Value.ToList();
            else
                ErrorMessage = getIngredientsResult.Error.Message;
        }
        catch
        {
            ErrorMessage = "An error occurred while retrieving the ingredients.";
        }
    }

    private async Task CreateRecipe()
    {
        var dialogParameters = new DialogParameters
        {
            { nameof(CreateRecipeDialog.Ingredients), GetIngredientResponses },
            { nameof(CreateRecipeDialog.Recipes), GetRecipeResponses }
        };

        var dialog = await dialogService.ShowAsync<CreateRecipeDialog>("Create Recipe", dialogParameters);
        var dialogResult = await dialog.Result;

        if (dialogResult is { Canceled: false })
            await LoadRecipes();
    }
}