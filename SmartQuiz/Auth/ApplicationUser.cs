using Microsoft.AspNetCore.Identity;

namespace SmartQuiz.Auth;

public sealed class ApplicationUser : IdentityUser
{
    public string? Picture { get; init; }

    public ICollection<IdentityUserLogin<string>> UserLogins { get; init; } = new List<IdentityUserLogin<string>>();
}