using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartQuiz.Client.Auth;

namespace SmartQuiz.Auth;

internal static class UserProfileEndpointBuilderExtensions
{
    extension(IEndpointRouteBuilder endpoints)
    {
        internal IEndpointConventionBuilder MapRegisterUser()
        {
            var group = endpoints.MapGroup("");

            group.MapPost("/register",
                async (HttpContext context, [FromBody] UserProfileRequests.RegisterUserRequest request,
                    UserManager<IdentityUser> userManager) =>
                {
                    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    {
                        return Results.BadRequest("Email and Password are required.");
                    }

                    var user = new IdentityUser { UserName = request.Email, Email = request.Email };
                    var result = await userManager.CreateAsync(user, request.Password);

                    return !result.Succeeded
                        ? Results.BadRequest(result.Errors)
                        : Results.Ok(new { Message = "User registered successfully" });
                });

            return group;
        }

        internal IEndpointConventionBuilder MapGetUserProfile()
        {
            var group = endpoints.MapGroup("user");

            group.MapGet("/profile",
                async (HttpContext context, UserManager<IdentityUser> userManager) =>
                {
                    var user = await userManager.GetUserAsync(context.User);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }

                    return Results.Ok(new UserProfileResponses.UserResponse()
                    {
                        Email = user.Email,
                        Id = user.Id
                    });
                });

            return group;
        }

        internal IEndpointConventionBuilder MapChangePassword()
        {
            var group = endpoints.MapGroup("user");

            group.MapPost("/change-password",
                async (HttpContext context, [FromBody] UserProfileRequests.ChangePasswordRequest request,
                    UserManager<IdentityUser> userManager) =>
                {
                    if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                        string.IsNullOrWhiteSpace(request.NewPassword))
                    {
                        return Results.BadRequest("Current password and new password are required.");
                    }

                    var user = await userManager.GetUserAsync(context.User);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }

                    var result =
                        await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                    return !result.Succeeded
                        ? Results.BadRequest(result.Errors)
                        : Results.Ok(new { Message = "Password changed successfully" });
                });

            return group;
        }
    }
}