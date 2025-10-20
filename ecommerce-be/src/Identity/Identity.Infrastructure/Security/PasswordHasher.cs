using System.Security.Cryptography;
using System.Text;
using Identity.Application.Abstractions.Security;

namespace Identity.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;        // 128-bit
    private const int KeySize = 32;        // 256-bit
    private const int Iter = 100_000;       // PBKDF2 iterations

    public (byte[] hash, byte[] salt) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, Iter, HashAlgorithmName.SHA256, KeySize);
        return (hash, salt);
    }

    public bool Verify(string password, byte[] hash, byte[] salt)
    {
        var test = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, Iter, HashAlgorithmName.SHA256, KeySize);

        Console.WriteLine($"Expected: {Convert.ToHexString(hash)}");
        Console.WriteLine($"Actual:   {Convert.ToHexString(test)}");

        return CryptographicOperations.FixedTimeEquals(hash, test);
    }
}
