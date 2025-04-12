using Microsoft.AspNetCore.Identity;

namespace BlogASP.API.Helpers
{
    public class PasswordHasherHelper
    {
        // Instance of the PasswordHasher class for hashing and verifying passwords
        private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

        // Method to hash a password
        // It uses the PasswordHasher to generate a hashed version of the provided password
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);  // 'null' is used here as we don't need a user-specific context for hashing
        }

        // Method to verify if the provided password matches the hashed password
        // It returns 'true' if the password matches, otherwise 'false'
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);

            // Return 'true' if the password verification is successful, otherwise 'false'
            return result == PasswordVerificationResult.Success;
        }
    }
}
