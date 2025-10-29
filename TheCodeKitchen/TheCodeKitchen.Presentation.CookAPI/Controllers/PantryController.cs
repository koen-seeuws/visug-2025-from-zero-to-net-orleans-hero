using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Pantry;
using TheCodeKitchen.Application.Contracts.Response.Pantry;
using TheCodeKitchen.Application.Contracts.Results;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Application.Validation.Pantry;
using TheCodeKitchen.Infrastructure.Security.Extensions;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class PantryController(
    IClusterClient clusterClient,
    IMemoryCache memoryCache,
    TakeFoodFromPantryValidator foodFromPantryValidator
) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> Inventory()
    {
        var id = Guid.Empty;
        var cacheId = $"ingredients-{id}";
        
        if (memoryCache.TryGetValue<Result<IEnumerable<GetIngredientResponse>>>(cacheId, out var cachedResult))
            return this.MatchActionResult(cachedResult!);
        
        var pantryGrain = clusterClient.GetGrain<IPantryGrain>(id);
        var result = await pantryGrain.GetIngredients();
        
        if (result.Succeeded)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
            memoryCache.Set(cacheId, result, cacheEntryOptions);
        }
        
        return this.MatchActionResult(result);
    }

    [HttpPost("{ingredient}/[action]")]
    public async Task<IActionResult> TakeFood([FromRoute] string ingredient)
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var request = new TakeFoodFromPantryRequest(ingredient, kitchen, cook);
        if (!foodFromPantryValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var pantryGrain = clusterClient.GetGrain<IPantryGrain>(Guid.Empty);
        var result = await pantryGrain.TakeFood(request);
        return this.MatchActionResult(result);
    }
}