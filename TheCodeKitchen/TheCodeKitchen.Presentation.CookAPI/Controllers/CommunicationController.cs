using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Application.Validation.Cook;
using TheCodeKitchen.Infrastructure.Security.Extensions;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers;

[Tags("Communication")]
[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class CommunicationController(
    IClusterClient clusterClient,
    SendMessageValidator sendMessageValidator,
    ConfirmMessageValidator confirmMessageValidator
) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<IActionResult> SendMessage(SendMessageRequest request)
    {
        if (!sendMessageValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);

        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var kitchenGrain = clusterClient.GetGrain<IKitchenGrain>(kitchen);
        var deliverMessageToKitchenRequest = new DeliverMessageRequest(cook, request.To, request.Content);
        var result = await kitchenGrain.DeliverMessage(deliverMessageToKitchenRequest);
        return this.MatchActionResult(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ReadMessages()
    {
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var cookGrain = clusterClient.GetGrain<ICookGrain>(kitchen, cook);
        var result = await cookGrain.ReadMessages();
        return this.MatchActionResult(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ConfirmMessage(ConfirmMessageRequest request)
    {
        if (!confirmMessageValidator.ValidateAndError(request, out var error)) return this.MatchActionResult(error);
        var kitchen = HttpContext.User.GetKitchenId();
        var cook = HttpContext.User.GetUsername();
        var cookGrain = clusterClient.GetGrain<ICookGrain>(kitchen, cook);
        var result = await cookGrain.ConfirmMessage(request);
        return this.MatchActionResult(result);
    }
}