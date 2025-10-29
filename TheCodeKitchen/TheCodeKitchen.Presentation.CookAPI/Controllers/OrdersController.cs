using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.KitchenOrder;
using TheCodeKitchen.Infrastructure.Security.Extensions;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class OrdersController(IClusterClient clusterClient) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> ViewOpen()
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var kitchenGrain = clusterClient.GetGrain<IKitchenGrain>(kitchen);
        var result = await kitchenGrain.GetOpenOrders();
        return this.MatchActionResult(result);
    }

    [HttpPost("{orderNumber:long}/[action]")]
    public async Task<IActionResult> Deliver([FromRoute] long orderNumber)
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var kitchenOrderGrain = clusterClient.GetGrain<IKitchenOrderGrain>(orderNumber, kitchen.ToString());
        var deliverFoodRequest = new DeliverFoodRequest(cook);
        var result = await kitchenOrderGrain.DeliverFood(deliverFoodRequest);
        return this.MatchActionResult(result);
    }

    [HttpPost("{orderNumber:long}/[action]")]
    public async Task<IActionResult> Complete([FromRoute] long orderNumber)
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var kitchenOrderGrain = clusterClient.GetGrain<IKitchenOrderGrain>(orderNumber, kitchen.ToString());
        var result = await kitchenOrderGrain.Complete();
        return this.MatchActionResult(result);
    }
}