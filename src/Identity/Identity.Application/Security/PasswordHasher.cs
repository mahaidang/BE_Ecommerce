using System.Security.Cryptography;
using System.Text;

namespace Identity.Application.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16;        // 128-bit
    private const int KeySize = 32;        // 256-bit
    private const int Iter = 100_000;   // PBKDF2 iterations

    public static (byte[] hash, byte[] salt) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, Iter, HashAlgorithmName.SHA256, KeySize);
        return (hash, salt);
    }

    public static bool Verify(string password, byte[] hash, byte[] salt)
    {
        var test = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, Iter, HashAlgorithmName.SHA256, KeySize);
        return CryptographicOperations.FixedTimeEquals(hash, test);
    }
}
