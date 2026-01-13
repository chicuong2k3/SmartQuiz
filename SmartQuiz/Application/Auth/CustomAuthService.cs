using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ActualLab.Fusion.Authentication;
using ActualLab.Fusion.Authentication.Services;
using Konscious.Security.Cryptography;
using NotificationModule;
using SmartQuiz.Client.Data.Commands;
using SmartQuiz.Client.Data.Services;
using SmartQuiz.Templates;

namespace SmartQuiz.Application.Auth;

public class CustomAuthService(
    IServiceProvider services,
    ICommander commander,
    IEmailSender emailSender,
    IOtpService otpService,
    IEmailTemplateService emailTemplateService)
    : DbServiceBase<ApplicationDbContext>(services), ICustomAuthService
{
    private const int MaxOtpAttempts = 5;
    private readonly Dictionary<string, int> _otpAttemptCounter = new();

    [CommandHandler]
    public virtual async Task<User?> SignInAsync(
        SignInCommand command,
        CancellationToken cancellationToken)
    {
        if (Invalidation.IsActive)
        {
            return null;
        }

        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);

        var dbUser = await FindUserByEmailAsync(command.Email, cancellationToken);

        if (dbUser == null)
            throw new InvalidOperationException("Email hoặc mật khẩu không đúng");

        var emailIdentityId = $"email/{command.Email}";
        var existingIdentity = await FindIdentityAsync(
            dbUser.Id,
            emailIdentityId,
            cancellationToken
        );

        if (existingIdentity == null)
        {
            throw new InvalidOperationException("Email hoặc mật khẩu không đúng");
        }


        var passwordHash = existingIdentity.Secret;

        if (!VerifyPassword(command.Password, passwordHash))
            throw new InvalidOperationException("Email hoặc mật khẩu không đúng");

        // Check if email is confirmed
        var isEmailConfirmed = dbUser.Claims.TryGetValue("EmailConfirmed", out var confirmed)
                               && confirmed == "true";
        if (!isEmailConfirmed)
        {
            // Resend OTP for verification
            await commander.Call(new SendConfirmationEmailCommand(
                command.Email,
                dbUser.Name
            ), cancellationToken);

            throw new InvalidOperationException("Email chưa được xác thực. Mã OTP mới đã được gửi đến email của bạn.");
        }

        var userIdentity = new UserIdentity("email", command.Email);
        var user = new User(dbUser.Id, dbUser.Name)
            .WithIdentity(userIdentity);

        foreach (var claim in dbUser.Claims)
        {
            user.WithClaim(claim.Key, claim.Value);
        }

        var signInCommand = new AuthBackend_SignIn(
            command.Session,
            user,
            userIdentity
        );

        await commander.Call(signInCommand, cancellationToken);

        return user;
    }

    [CommandHandler]
    public virtual async Task<User> RegisterAsync(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        if (Invalidation.IsActive)
        {
            return new User("", "");
        }

        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);

        var existingUser = await FindUserByEmailAsync(command.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "Email đã được sử dụng để đăng ký."
            );
        }

        existingUser = new DbUser<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Name = command.FullName,
            Version = 0
        };

        var claims = new Dictionary<string, string>
        {
            [ClaimTypes.Email] = command.Email,
            [ClaimTypes.Name] = command.FullName
        };
        existingUser.ClaimsJson = JsonSerializer.Serialize(claims);

        var emailIdentityId = $"email/{command.Email}";
        var existingIdentity = await FindIdentityAsync(existingUser.Id, emailIdentityId, cancellationToken);

        if (existingIdentity != null)
        {
            throw new InvalidOperationException(
                "Email đã được sử dụng để đăng ký."
            );
        }

        var dbIdentity = new DbUserIdentity<string>
        {
            Id = $"email/{command.Email}",
            DbUserId = existingUser.Id,
            Secret = HashPassword(command.Password)
        };
        dbContext.Set<DbUser<string>>().Add(existingUser);
        dbContext.Set<DbUserIdentity<string>>().Add(dbIdentity);

        await dbContext.SaveChangesAsync(cancellationToken);

        var user = new User(existingUser.Id, existingUser.Name)
            .WithIdentity(new UserIdentity("email", command.Email));
        foreach (var claim in claims)
        {
            user.WithClaim(claim.Key, claim.Value);
        }

        // Send confirmation email
        await commander.Call(new SendConfirmationEmailCommand(
            command.Email,
            command.FullName
        ), cancellationToken);

        return user;
    }

    [CommandHandler]
    public virtual async Task SendConfirmationEmailAsync(
        SendConfirmationEmailCommand command,
        CancellationToken cancellationToken)
    {
        if (Invalidation.IsActive)
            return;

        var otp = otpService.GenerateOtp(command.Email);
        var emailBody = emailTemplateService.GetOtpEmailTemplate(command.FullName, otp);

        await emailSender.SendEmailAsync(
            command.Email,
            "Mã xác nhận OTP - Đăng ký tài khoản mới",
            emailBody
        );
    }

    [CommandHandler]
    public virtual async Task<bool> ConfirmEmailAsync(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        if (Invalidation.IsActive)
            return false;

        if (_otpAttemptCounter.TryGetValue(command.Email, out var attempts) && attempts >= MaxOtpAttempts)
        {
            throw new InvalidOperationException("Thử quá nhiều lần. Vui lòng thử lại sau.");
        }

        var isValid = otpService.ValidateOtp(command.Email, command.Otp);

        if (!isValid)
        {
            _otpAttemptCounter[command.Email] = attempts + 1;
            return false;
        }

        _otpAttemptCounter.Remove(command.Email);

        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);

        var user = await FindUserByEmailAsync(command.Email, cancellationToken);

        if (user == null)
            return false;

        var claims = user.Claims.ToDictionary(c => c.Key, c => c.Value);
        claims["EmailConfirmed"] = "true";
        user.ClaimsJson = JsonSerializer.Serialize(claims);

        dbContext.Set<DbUser<string>>().Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        otpService.InvalidateOtp(command.Email);

        return true;
    }

    [CommandHandler]
    public virtual async Task SendPasswordResetOtpAsync(
        SendPasswordResetOtpCommand command,
        CancellationToken cancellationToken)
    {
        if (Invalidation.IsActive)
            return;

        var user = await FindUserByEmailAsync(command.Email, cancellationToken);
        if (user == null)
        {
            // Don't reveal if user exists or not for security
            return;
        }

        var otp = otpService.GenerateOtp(command.Email, OtpPurpose.PasswordReset);
        var emailBody = emailTemplateService.GetPasswordResetOtpEmailTemplate(otp);

        await emailSender.SendEmailAsync(
            command.Email,
            "Mã xác nhận OTP - Đặt lại mật khẩu",
            emailBody
        );
    }

    [CommandHandler]
    public virtual Task<bool> VerifyPasswordResetOtpAsync(
        VerifyPasswordResetOtpCommand command,
        CancellationToken cancellationToken)
    {
        if (Invalidation.IsActive)
            return Task.FromResult(false);

        if (_otpAttemptCounter.TryGetValue(command.Email, out var attempts) && attempts >= MaxOtpAttempts)
        {
            throw new InvalidOperationException("Thử quá nhiều lần. Vui lòng thử lại sau.");
        }

        var isValid = otpService.ValidateOtp(command.Email, command.Otp, OtpPurpose.PasswordReset);

        if (!isValid)
        {
            _otpAttemptCounter[command.Email] = attempts + 1;
            return Task.FromResult(false);
        }

        _otpAttemptCounter.Remove(command.Email);
        return Task.FromResult(true);
    }

    [CommandHandler]
    public virtual async Task<bool> ResetPasswordAsync(
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        if (Invalidation.IsActive)
            return false;

        if (!otpService.ValidateOtp(command.Email, command.Otp, OtpPurpose.PasswordReset))
            return false;

        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);

        var user = await FindUserByEmailAsync(command.Email, cancellationToken);
        if (user == null)
            return false;

        var emailIdentityId = $"email/{command.Email}";
        var identity = await FindIdentityAsync(user.Id, emailIdentityId, cancellationToken);

        if (identity == null)
            return false;

        // Update password
        identity.Secret = HashPassword(command.NewPassword);
        dbContext.Set<DbUserIdentity<string>>().Update(identity);
        await dbContext.SaveChangesAsync(cancellationToken);

        otpService.InvalidateOtp(command.Email, OtpPurpose.PasswordReset);

        return true;
    }

    private string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = 65536,
            Iterations = 4,
            DegreeOfParallelism = 1
        };

        var hash = argon2.GetBytes(32);

        return $"argon2id$v=19$m=65536,t=4,p=1${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    private async Task<DbUser<string>?> FindUserByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);

        return await dbContext.Set<DbUser<string>>()
            .FromSql($@"
                SELECT *
                FROM ""Users""
                WHERE (""ClaimsJson""::jsonb) ->> {ClaimTypes.Email} = {email}
            ")
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<DbUserIdentity<string>?> FindIdentityAsync(
        string userId,
        string identityId,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);

        return await dbContext.Set<DbUserIdentity<string>>()
            .FirstOrDefaultAsync(
                i => i.DbUserId == userId && i.Id == identityId,
                cancellationToken
            );
    }

    private bool VerifyPassword(string password, string? storedHash)
    {
        Console.WriteLine(storedHash);
        Console.WriteLine(password);
        if (string.IsNullOrWhiteSpace(storedHash))
            return false;

        var parts = storedHash.Split('$');
        if (parts.Length != 5 || parts[0] != "argon2id")
            return false;

        var paramPart = parts[2];
        var parameters = paramPart.Split(',');

        int memory = int.Parse(parameters[0].Split('=')[1]);
        int iterations = int.Parse(parameters[1].Split('=')[1]);
        int parallelism = int.Parse(parameters[2].Split('=')[1]);

        var salt = Convert.FromBase64String(parts[3]);
        var expectedHash = Convert.FromBase64String(parts[4]);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = memory,
            Iterations = iterations,
            DegreeOfParallelism = parallelism
        };

        var actualHash = argon2.GetBytes(expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}