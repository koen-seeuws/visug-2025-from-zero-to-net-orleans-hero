using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Game;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Application.Validation.Game;

namespace TheCodeKitchen.Presentation.ManagementAPI.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class GameController(
    IClusterClient client,
    CreateGameValidator createGameValidator,
    UpdateGameValidator updateGameValidator
) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGameRequest request)
    {
        if (!createGameValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var gameManagementGrain = client.GetGrain<IGameManagementGrain>(Guid.Empty);
        var result = await gameManagementGrain.CreateGame(request);
        return this.MatchActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var gameManagementGrain = client.GetGrain<IGameManagementGrain>(Guid.Empty);
        var result = await gameManagementGrain.GetGames();
        return this.MatchActionResult(result);
    }

    [HttpGet("{gameId}/[action]")]
    public async Task<IActionResult> Get(Guid gameId)
    {
        var gameGrain = client.GetGrain<IGameGrain>(gameId);
        var result = await gameGrain.GetGame();
        return this.MatchActionResult(result);
    }

    [HttpPut("{gameId}/[action]")]
    public async Task<IActionResult> Start(Guid gameId)
    {
        var gameGrain = client.GetGrain<IGameGrain>(gameId);
        var result = await gameGrain.StartGame();
        return this.MatchActionResult(result);
    }

    [HttpPut("{gameId}/[action]")]
    public async Task<IActionResult> PauseOrUnpause(Guid gameId)
    {
        var gameGrain = client.GetGrain<IGameGrain>(gameId);
        var result = await gameGrain.PauseOrUnpauseGame();
        return this.MatchActionResult(result);
    }

    [HttpPut("{gameId}/[action]")]
    public async Task<IActionResult> Update(Guid gameId, [FromBody] UpdateGameRequest request)
    {
        if (!updateGameValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var gameGrain = client.GetGrain<IGameGrain>(gameId);
        var result = await gameGrain.UpdateGame(request);
        return this.MatchActionResult(result);
    }

    [HttpPost("{gameId}/[action]")]
    public async Task<IActionResult> NextMoment(Guid gameId)
    {
        var gameGrain = client.GetGrain<IGameGrain>(gameId);
        var result = await gameGrain.NextMoment();
        return this.MatchActionResult(result);
    }

    [HttpPost("{gameId}/[action]")]
    public async Task<IActionResult> Reset(Guid gameId)
    {
        var gameGrain = client.GetGrain<IGameGrain>(gameId);
        var result = await gameGrain.ResetGame();
        return this.MatchActionResult(result);
    }
}