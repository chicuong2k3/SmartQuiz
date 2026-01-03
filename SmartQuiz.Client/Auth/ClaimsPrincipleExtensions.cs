using System.Security.Claims;

namespace SmartQuiz.Client.Auth
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? string.Empty;
        }
    }
}