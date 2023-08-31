using System.Security.Cryptography;
using System.Text;

namespace JWTGenerator.Services
{
    public class HelperMethods
    {
        public void CreateHasedPassword(string password, out byte[] hashedPassword, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyHasedPassword(string password, byte[] hashedPassword, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var ComputedHashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return ComputedHashedPassword == hashedPassword;
            }
        }
    }
}
