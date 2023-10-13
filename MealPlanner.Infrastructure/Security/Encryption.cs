using System.Security.Cryptography;

namespace MealPlanner.Infrastructure.Security;

public class Encryption
{
    public byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];

        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    public byte[] HashPassword(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(32);
        }
    }
}
