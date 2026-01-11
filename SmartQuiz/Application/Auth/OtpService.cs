using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace SmartQuiz.Application.Auth;

public enum OtpPurpose
{
    EmailConfirmation,
    PasswordReset
}

public interface IOtpService
{
    string GenerateOtp(string email, OtpPurpose purpose = OtpPurpose.EmailConfirmation);
    bool ValidateOtp(string email, string otp, OtpPurpose purpose = OtpPurpose.EmailConfirmation);
    void InvalidateOtp(string email, OtpPurpose purpose = OtpPurpose.EmailConfirmation);
}

public record OtpData(string Otp, DateTime ExpiresAt);

public class OtpService : IOtpService
{
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(5);
    private readonly ConcurrentDictionary<string, OtpData> _otpStore = new();

    public string GenerateOtp(string email, OtpPurpose purpose = OtpPurpose.EmailConfirmation)
    {
        var key = GetKey(email, purpose);
        var otp = GenerateRandomOtp();
        var data = new OtpData(otp, DateTime.UtcNow.Add(OtpLifetime));

        _otpStore.AddOrUpdate(key, data, (_, _) => data);

        CleanupExpiredOtps();

        return otp;
    }

    public bool ValidateOtp(string email, string otp, OtpPurpose purpose = OtpPurpose.EmailConfirmation)
    {
        var key = GetKey(email, purpose);

        if (!_otpStore.TryGetValue(key, out var data))
            return false;

        if (data.ExpiresAt < DateTime.UtcNow)
        {
            _otpStore.TryRemove(key, out _);
            return false;
        }

        return string.Equals(data.Otp, otp, StringComparison.OrdinalIgnoreCase);
    }

    public void InvalidateOtp(string email, OtpPurpose purpose = OtpPurpose.EmailConfirmation)
    {
        var key = GetKey(email, purpose);
        _otpStore.TryRemove(key, out _);
    }

    private static string GetKey(string email, OtpPurpose purpose)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return $"{purpose}:{normalizedEmail}";
    }

    private static string GenerateRandomOtp()
    {
        var bytes = RandomNumberGenerator.GetBytes(4);
        var number = BitConverter.ToUInt32(bytes, 0) % 1000000;
        return number.ToString("D6");
    }

    private void CleanupExpiredOtps()
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _otpStore
            .Where(kvp => kvp.Value.ExpiresAt < now)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _otpStore.TryRemove(key, out _);
        }
    }
}