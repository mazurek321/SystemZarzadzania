using System.Security.Claims;


namespace Infrastructure.Context;
public static class ClaimsHelper
{
    public static Guid? GetUserIdFromClaims(ClaimsPrincipal? user)
    {
        var userId = user?.FindFirst("userid")?.Value;
        return Guid.TryParse(userId, out var id) ? id : (Guid?)null;
    }
}