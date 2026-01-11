using ActualLab.Fusion.Authentication;

namespace SmartQuiz.Client.Data.Services;

public interface ICustomAuthService : IComputeService
{
    [CommandHandler]
    Task<User?> SignInAsync(
        SignInCommand command,
        CancellationToken cancellationToken);

    [CommandHandler]
    Task<User> RegisterAsync(
        RegisterCommand command,
        CancellationToken cancellationToken);

    [CommandHandler]
    Task<bool> ConfirmEmailAsync(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken);

    [CommandHandler]
    Task SendConfirmationEmailAsync(
        SendConfirmationEmailCommand command,
        CancellationToken cancellationToken);

    [CommandHandler]
    Task SendPasswordResetOtpAsync(
        SendPasswordResetOtpCommand command,
        CancellationToken cancellationToken);

    [CommandHandler]
    Task<bool> VerifyPasswordResetOtpAsync(
        VerifyPasswordResetOtpCommand command,
        CancellationToken cancellationToken);

    [CommandHandler]
    Task<bool> ResetPasswordAsync(
        ResetPasswordCommand command,
        CancellationToken cancellationToken);
}