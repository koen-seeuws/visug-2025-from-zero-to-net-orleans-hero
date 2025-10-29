using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Response.CookBook;
using TheCodeKitchen.Application.Contracts.Results;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers;

[Tags("Cook Book")]
[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class CookBookController(IClusterClient clusterClient, IMemoryCache memoryCache) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> Read()
    {
        var id = Guid.Empty;
        var cacheId = $"recipes-{id}";
        
        if (memoryCache.TryGetValue<Result<IEnumerable<GetRecipeResponse>>>(cacheId, out var cachedResult))
            return this.MatchActionResult(cachedResult!);
        
        var cookBookGrain = clusterClient.GetGrain<ICookBookGrain>(id);
        var result = await cookBookGrain.GetRecipes();

        if (result.Succeeded)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
            memoryCache.Set(cacheId, result, cacheEntryOptions);
        }
        
        return this.MatchActionResult(result);
    }
}