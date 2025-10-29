using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Errors;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Infrastructure.Security;
using TheCodeKitchen.Presentation.API.Cook.Models;
using TheCodeKitchen.Presentation.API.Cook.Validators;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class KitchenController(
    IClusterClient client,
    IPasswordHashingService passwordHashingService,
    ISecurityTokenService securityTokenService,
    AuthenticationRequestValidator authenticationRequestValidator
) : ControllerBase
{
    [HttpPost("{code}/[action]")]
    public async Task<IActionResult> Join([FromRoute] string code, [FromBody] AuthenticationRequest request)
    {
        if (!authenticationRequestValidator.ValidateAndError(request, out var error))
            return this.MatchActionResult(error!);

        var kitchenCodeIndexGrain = client.GetGrain<IKitchenManagementGrain>(Guid.Empty);

        var passwordHash = passwordHashingService.HashPassword(request.Password);

        var joinKitchenRequest = new JoinKitchenRequest(
            request.Username,
            passwordHash,
            code
        );

        var joinKitchenResult = await kitchenCodeIndexGrain.JoinKitchen(joinKitchenRequest);

        if (!joinKitchenResult.Succeeded)
            return this.MatchActionResult(joinKitchenResult);

        if (
            !joinKitchenResult.Value.isNewCook &&
            !passwordHashingService.VerifyHashedPassword(joinKitchenResult.Value.PasswordHash, request.Password)
        )
            return this.MatchActionResult(new UnauthorizedError("Invalid password"));

        var token = securityTokenService.GeneratePlayerToken(
            joinKitchenResult.Value.GameId,
            joinKitchenResult.Value.KitchenId,
            joinKitchenResult.Value.Username
        );

        var response = new AuthenticationResponse(token);

        return Ok(response);
    }
}