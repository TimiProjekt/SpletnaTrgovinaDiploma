using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class UserClaimsPrincipalExtensionMethod
    {
        public static bool IsLoggedIn(this ClaimsPrincipal user)
            => user.Identity?.IsAuthenticated ?? false;

        public static bool IsUserAdmin(this ClaimsPrincipal user)
            => user.IsLoggedIn() && user.IsInRole(UserRoles.Admin);

        public static string GetUserId(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static ApplicationUser GetApplicationUserFromEmail(this UserManager<ApplicationUser> userManager, string email)
            => userManager.FindByNameAsync(email).Result;

        public static ApplicationUser GetApplicationUser(this ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
        {
            var email = userManager.GetUserName(user);
            return userManager.GetApplicationUserFromEmail(email);
        }

    }
}
