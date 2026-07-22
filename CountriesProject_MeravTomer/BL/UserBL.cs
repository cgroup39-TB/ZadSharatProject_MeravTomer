using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ServerSideCountriesProject_MeravTomer.DAL;
using ServerSideCountriesProject_MeravTomer.Models;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class UserBL
    {
        private const int MinPasswordLength = 6;
        private readonly DBUserServices dbUserServices = new DBUserServices();

        public User Register(User newUser)
        {
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.Name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(newUser.Email) || !IsValidEmail(newUser.Email))
                throw new ArgumentException("A valid email is required.");

            if (string.IsNullOrWhiteSpace(newUser.Password) || newUser.Password.Length < MinPasswordLength)
                throw new ArgumentException($"Password must be at least {MinPasswordLength} characters long.");

            if (dbUserServices.GetUserByEmail(newUser.Email) != null)
                throw new ArgumentException("Email is already registered.");

            // The User(userId, name, email, password) constructor forces IsActive=true, IsAdmin=false,
            // CanShare=true, so a client can never register itself as an admin.
            User userToInsert = new User(0, newUser.Name, newUser.Email, HashPassword(newUser.Password));
            userToInsert.UserId = dbUserServices.InsertUser(userToInsert);

            return SanitizeForResponse(userToInsert);
        }

        public User Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            User user = dbUserServices.GetUserByEmail(email);

            if (user == null || !VerifyPassword(password, user.Password))
                return null;

            if (!user.IsActive)
                throw new UnauthorizedAccessException("This account has been deactivated.");

            return SanitizeForResponse(user);
        }

        public User GetUserById(int userId)
        {
            User user = dbUserServices.GetUserById(userId);
            return user == null ? null : SanitizeForResponse(user);
        }

        public User UpdateProfile(int userId, string name, string email)
        {
            User existingUser = dbUserServices.GetUserById(userId);
            if (existingUser == null)
                return null;

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new ArgumentException("A valid email is required.");

            User userWithSameEmail = dbUserServices.GetUserByEmail(email);
            if (userWithSameEmail != null && userWithSameEmail.UserId != userId)
                throw new ArgumentException("Email is already registered to another user.");

            dbUserServices.UpdateUserProfile(userId, name, email);

            existingUser.Name = name;
            existingUser.Email = email;
            return SanitizeForResponse(existingUser);
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            User existingUser = dbUserServices.GetUserById(userId);
            if (existingUser == null)
                return false;

            if (!VerifyPassword(currentPassword, existingUser.Password))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < MinPasswordLength)
                throw new ArgumentException($"New password must be at least {MinPasswordLength} characters long.");

            dbUserServices.UpdateUserPassword(userId, HashPassword(newPassword));
            return true;
        }

        // Returns null when the user itself doesn't exist, and an empty list when the user
        // simply has no languages set - the two are not the same thing.
        public List<UserLanguage> GetUserLanguages(int userId)
        {
            return dbUserServices.GetUserById(userId) == null ? null : dbUserServices.GetUserLanguages(userId);
        }

        public bool UpdateUserLanguages(int userId, List<UserLanguage> languages)
        {
            if (dbUserServices.GetUserById(userId) == null)
                return false;

            if (languages != null && languages.Any(l => l.LanguageId <= 0 || string.IsNullOrWhiteSpace(l.ProficiencyLevel)))
                throw new ArgumentException("Each language must have a valid LanguageId and ProficiencyLevel.");

            dbUserServices.DeleteUserLanguages(userId);

            if (languages != null)
            {
                foreach (UserLanguage language in languages)
                {
                    dbUserServices.InsertUserLanguage(userId, language.LanguageId, language.ProficiencyLevel);
                }
            }

            return true;
        }

        public List<Region> GetPreferredRegions(int userId)
        {
            return dbUserServices.GetUserById(userId) == null ? null : dbUserServices.GetPreferredRegions(userId);
        }

        public bool UpdatePreferredRegions(int userId, List<int> regionIds)
        {
            if (dbUserServices.GetUserById(userId) == null)
                return false;

            dbUserServices.DeletePreferredRegions(userId);

            if (regionIds != null)
            {
                foreach (int regionId in regionIds)
                {
                    dbUserServices.InsertPreferredRegion(userId, regionId);
                }
            }

            return true;
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private static User SanitizeForResponse(User user)
        {
            user.Password = null;
            return user;
        }

        private static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, 100_000, HashAlgorithmName.SHA256, 32);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            string[] parts = storedHash.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);
            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, 100_000, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
