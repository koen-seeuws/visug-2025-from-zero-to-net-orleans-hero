using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Application.Validation.Kitchen;

namespace TheCodeKitchen.Presentation.ManagementAPI.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class KitchenController(
    IClusterClient client,
    CreateKitchenValidator createKitchenValidator
) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKitchenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!createKitchenValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var gameGrain = client.GetGrain<IGameGrain>(request.GameId);
        var result = await gameGrain.CreateKitchen(request);
        return this.MatchActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetKitchens([FromQuery] Guid gameId, CancellationToken cancellationToken = default)
    {
        var gameGrain = client.GetGrain<IGameGrain>(gameId);
        var result = await gameGrain.GetKitchens();
        return this.MatchActionResult(result);
    }
}