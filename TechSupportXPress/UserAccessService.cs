using System.Security.Claims;

namespace TechSupportXPress
{
    public static class UserAccessService
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentloggedinUser = user;
                if (currentloggedinUser != null)
                {
                    return currentloggedinUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
