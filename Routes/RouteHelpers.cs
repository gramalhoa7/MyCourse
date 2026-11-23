using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MyCourse.Models;

namespace MyCourse.Routes;

public static class RouteHelpers
{
    // Id de quem está logado, lido do token. Nunca confie num UserId enviado pelo front.
    public static Guid? GetCurrentUserId(this UserManager<Users> userManager, ClaimsPrincipal user)
    {
        var text = userManager.GetUserId(user);
        return Guid.TryParse(text, out var id) ? id : null;
    }

    // Administrador ou funcionário
    public static bool IsStaff(this ClaimsPrincipal user) =>
        user.IsInRole(Roles.Admin) || user.IsInRole(Roles.Staff);
}
