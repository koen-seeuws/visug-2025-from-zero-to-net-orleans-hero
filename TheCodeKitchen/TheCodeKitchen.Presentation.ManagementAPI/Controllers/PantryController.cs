using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Pantry;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Application.Validation.Pantry;

namespace TheCodeKitchen.Presentation.ManagementAPI.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class PantryController(
    IClusterClient clusterClient,
    CreateIngredientValidator createIngredientValidator
) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetIngredients()
    {
        var pantryGrain = clusterClient.GetGrain<IPantryGrain>(Guid.Empty);
        var result = await pantryGrain.GetIngredients();
        return this.MatchActionResult(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientRequest request)
    {
        if (!createIngredientValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var pantryGrain = clusterClient.GetGrain<IPantryGrain>(Guid.Empty);
        var result = await pantryGrain.CreateIngredient(request);
        return this.MatchActionResult(result);
    }
}