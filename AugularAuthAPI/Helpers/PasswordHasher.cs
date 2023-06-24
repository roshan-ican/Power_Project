using System.Security.Cryptography;
namespace AugularAuthAPI.Helpers
{
    public class PasswordHasher
    {
        // Instantiate the random number generator
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        // Define the size of the salt and hashed password
        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 20;

        // Define the number of iterations for key derivation
        private static readonly int Iterations = 10000;

        // Hashes a given password
        public static string HashPassword(string password)
        {
            byte[] salt;

            // Generate a random salt
            rng.GetBytes(salt = new byte[SaltSize]);

            // Perform key derivation using the password, salt, and iterations
            var key = new Rfc2898DeriveBytes(password, salt, Iterations);

            // Generate the hashed password
            var hash = key.GetBytes(HashSize);

            // Combine the salt and hashed password into a single byte array
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert the byte array to a base64-encoded string
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Return the hashed password as a string
            return base64Hash;
        }


        //To verify the password
        public static bool VerifyPassword(string password, string base64Hash)
        {
            // Convert the base64-encoded hashed password to a byte array
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Extract the salt from the byte array
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Perform key derivation using the provided password and extracted salt
            var key = new Rfc2898DeriveBytes(password, salt, Iterations);

            // Generate a hash from the derived key
            byte[] hash = key.GetBytes(HashSize);

            // Compare the generated hash with the stored hash
             for (var i = 0; i < HashSize; i++)
            {
                // If any byte of the generated hash differs from the stored hash, the passwords don't match
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }

            // All bytes of the generated hash match the stored hash, indicating that the passwords match
            return true;
        }

    }
}
