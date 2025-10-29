using TheCodeKitchen.Application.Contracts.Models.Recipe;
using TheCodeKitchen.Application.Contracts.Response.CookBook;

namespace TheCodeKitchen.Application.Business.Mapping;

public sealed class RecipeMapping : Profile
{
    public RecipeMapping()
    {
        CreateMap<Recipe, CreateRecipeResponse>();
        CreateMap<Recipe, GetRecipeResponse>();
        CreateMap<RecipeIngredient, RecipeIngredientDto>();
        CreateMap<RecipeStep, RecipeStepDto>();
        
        CreateMap<GetRecipeResponse, Recipe>();
        CreateMap<RecipeIngredientDto, RecipeIngredient>();
        CreateMap<RecipeStepDto, RecipeStep>();
    }
}