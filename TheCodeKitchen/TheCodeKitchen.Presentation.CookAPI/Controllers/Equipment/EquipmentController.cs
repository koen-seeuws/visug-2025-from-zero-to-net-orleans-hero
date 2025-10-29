using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Equipment;
using TheCodeKitchen.Infrastructure.Security.Extensions;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[ApiController]
[Route("Equipment/[controller]")]
[Authorize]
public abstract class EquipmentController(IClusterClient clusterClient, string equipmentType) : ControllerBase
{
    [HttpPost("{number:int}/[action]")]
    public async Task<IActionResult> AddFood(int number)
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var equipmentGrainIdExtension = EquipmentGrainId.Create(equipmentType, number);
        var grain = clusterClient.GetGrain<IEquipmentGrain>(kitchen, equipmentGrainIdExtension);
        var addFoodRequest = new AddFoodRequest(cook);
        var result = await grain.AddFood(addFoodRequest);
        return this.MatchActionResult(result);
    }

    [HttpPost("{number:int}/[action]")]
    public async Task<IActionResult> TakeFood(int number)
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var equipmentGrainIdExtension = EquipmentGrainId.Create(equipmentType, number);
        var grain = clusterClient.GetGrain<IEquipmentGrain>(kitchen, equipmentGrainIdExtension);
        var takeFoodRequest =  new TakeFoodFromEquipmentRequest(cook);
        var result = await grain.TakeFood(takeFoodRequest);
        return this.MatchActionResult(result);
    }
}