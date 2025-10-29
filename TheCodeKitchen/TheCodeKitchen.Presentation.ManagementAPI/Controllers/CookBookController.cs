using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.CookBook;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Application.Validation.CookBook;

namespace TheCodeKitchen.Presentation.ManagementAPI.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class CookBookController(
    IClusterClient clusterClient,
    CreateRecipeValidator createRecipeValidator
) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetRecipes()
    {
        var cookBookGrain = clusterClient.GetGrain<ICookBookGrain>(Guid.Empty);
        var result = await cookBookGrain.GetRecipes();
        return this.MatchActionResult(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeRequest request)
    {
        if (!createRecipeValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var cookBookGrain = clusterClient.GetGrain<ICookBookGrain>(Guid.Empty);
        var result = await cookBookGrain.CreateRecipe(request);
        return this.MatchActionResult(result);
    }
}