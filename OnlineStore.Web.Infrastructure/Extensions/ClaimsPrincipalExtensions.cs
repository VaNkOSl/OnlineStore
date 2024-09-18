namespace OnlineStore.Web.Infrastructure.Extensions;

using System.Security.Claims;
using static OnlineStore.Commons.GeneralApplicationConstants;
public static class ClaimsPrincipalExtensions
{
    public static string? GetId(this ClaimsPrincipal user)
    {
        var claim = user?.FindFirst(ClaimTypes.NameIdentifier);
        return claim?.Value;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(AdminRoleName);
    }
}
