using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Application.Validation.Cook;
using TheCodeKitchen.Infrastructure.Security.Extensions;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers;

[Tags("Timers")]
[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class TimerController(
    IClusterClient clusterClient,
    SetTimerValidator setTimerValidator,
    StopTimerValidator stopTimerValidator
) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Set(SetTimerRequest request)
    {
        if (!setTimerValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var cookGrain = clusterClient.GetGrain<ICookGrain>(kitchen, cook);
        var result = await cookGrain.SetTimer(request);
        return this.MatchActionResult(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Get()
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var cookGrain = clusterClient.GetGrain<ICookGrain>(kitchen, cook);
        var result = await cookGrain.GetTimers();
        return this.MatchActionResult(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Stop(StopTimerRequest request)
    {
        if (!stopTimerValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var cookGrain = clusterClient.GetGrain<ICookGrain>(kitchen, cook);
        var result = await cookGrain.StopTimer(request);
        return this.MatchActionResult(result);
    }
}