using System.Security.Claims;

namespace TheCodeKitchen.Infrastructure.Security.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetClaim(this ClaimsPrincipal principal, string claimType)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var claim = principal.FindFirst(claimType);
        
        if (claim is null)
            throw new InvalidOperationException($"{claimType} claim not found.");
        
        return claim.Value;
    }

    public static Guid GetGuidClaim(this ClaimsPrincipal principal, string claimType)
    {
        var claimValue = GetClaim(principal, claimType);
        
        if (!Guid.TryParse(claimValue, out var guid))
            throw new InvalidOperationException($"{claimType} claim is not a valid GUID.");
        
        return guid;
    }
}