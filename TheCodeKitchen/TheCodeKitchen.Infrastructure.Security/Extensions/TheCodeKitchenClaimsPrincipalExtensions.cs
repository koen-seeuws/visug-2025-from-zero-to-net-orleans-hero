using System.Security.Claims;

namespace TheCodeKitchen.Infrastructure.Security.Extensions;

public static class TheCodeKitchenClaimsPrincipalExtensions
{
    public static Guid GetGameId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.GetGuidClaim(TheCodeKitchenClaimTypes.GameId);

    public static Guid GetKitchenId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.GetGuidClaim(TheCodeKitchenClaimTypes.KitchenId);

    public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.GetClaim(TheCodeKitchenClaimTypes.Username);
}