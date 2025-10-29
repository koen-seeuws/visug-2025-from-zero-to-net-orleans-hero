using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Infrastructure.Security.Extensions;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers;

[Tags("Trash")]
[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class TrashController(IClusterClient clusterClient) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<IActionResult> ThrowAway()
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var cookGrain = clusterClient.GetGrain<ICookGrain>(kitchen, cook);
        var result = await cookGrain.ThrowFoodAway();
        return this.MatchActionResult(result);
    }
}