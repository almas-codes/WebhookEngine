using System.Security.Cryptography;
using System.Text;

namespace WebhookEngine.Core.Signing;

public static class HmacSigner
{
    public static string GenerateSignature(string payload, string secret)
    {
        var encoding = new UTF8Encoding();
        var keyBytes = encoding.GetBytes(secret);
        var messageBytes = encoding.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);
        
        return Convert.ToBase64String(hashBytes);
    }

    public static bool Verify(string payload, string signature, string secret)
    {
        var expectedSignature = GenerateSignature(payload, secret);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(signature),
            Convert.FromBase64String(expectedSignature)
        );
    }
}
