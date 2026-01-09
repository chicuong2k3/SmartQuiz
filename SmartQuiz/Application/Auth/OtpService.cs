using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace SmartQuiz.Application.Auth;

public interface IOtpService
{
    string GenerateOtp(string email);
    bool ValidateOtp(string email, string otp);
    void InvalidateOtp(string email);
}

public record OtpData(string Otp, DateTime ExpiresAt);

public class OtpService : IOtpService
{
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(5);
    private readonly ConcurrentDictionary<string, OtpData> _otpStore = new();

    public string GenerateOtp(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var otp = GenerateRandomOtp();
        var data = new OtpData(otp, DateTime.UtcNow.Add(OtpLifetime));

        _otpStore.AddOrUpdate(normalizedEmail, data, (_, _) => data);

        CleanupExpiredOtps();

        return otp;
    }

    public bool ValidateOtp(string email, string otp)
    {
        var normalizedEmail = email.ToLowerInvariant();

        if (!_otpStore.TryGetValue(normalizedEmail, out var data))
            return false;

        if (data.ExpiresAt < DateTime.UtcNow)
        {
            _otpStore.TryRemove(normalizedEmail, out _);
            return false;
        }

        return string.Equals(data.Otp, otp, StringComparison.OrdinalIgnoreCase);
    }

    public void InvalidateOtp(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        _otpStore.TryRemove(normalizedEmail, out _);
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