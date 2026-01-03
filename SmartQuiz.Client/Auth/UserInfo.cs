using System.Security.Claims;

namespace SmartQuiz.Client.Auth;

public class UserInfo
{
    public string Id { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? Picture { get; init; }

    public static UserInfo? FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null)
        {
            return null;
        }

        var email = principal.FindFirst("email")?.Value;

        var name = principal.FindFirst("name")?.Value
                   ?? principal.FindFirst(ClaimTypes.GivenName)?.Value
                   ?? principal.FindFirst(ClaimTypes.Surname)?.Value;

        var picture = principal.FindFirst("picture")?.Value;

        return new UserInfo
        {
            Id = idClaim.Value,
            Email = email,
            Name = name,
            Picture = picture
        };
    }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var claims = new List<Claim>();

        if (!string.IsNullOrEmpty(Id))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Id));
        if (!string.IsNullOrEmpty(Email))
            claims.Add(new Claim("email", Email));
        if (!string.IsNullOrEmpty(Name))
            claims.Add(new Claim("name", Name));
        if (!string.IsNullOrEmpty(Picture))
            claims.Add(new Claim("picture", Picture));

        var identity = new ClaimsIdentity(claims, "oidc");
        return new ClaimsPrincipal(identity);
    }
}