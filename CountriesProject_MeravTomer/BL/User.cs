using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>
    /// Business-logic representation of an application user, and the entry point for
    /// authentication, profile management, admin permission changes, preferences and
    /// wanted-country list operations. Authorization checks (e.g. "only admins can...",
    /// "a user can only update his own profile") live here rather than in the controllers.
    /// </summary>
    public class User
    {
        private int userId;
        private string name;
        private string email;
        private string password;
        private bool isActive;
        private bool isAdmin;
        private bool canShare;

        /// <summary>Creates an empty user instance (used by model binding/deserialization).</summary>
        public User()
        {
        }

        /// <summary>Creates a fully-populated user, including active/admin/share flags.</summary>
        public User(
            int userId,
            string name,
            string email,
            string password,
            bool isActive,
            bool isAdmin,
            bool canShare)
        {
            UserId = userId;
            Name = name;
            Email = email;
            Password = password;
            IsActive = isActive;
            IsAdmin = isAdmin;
            CanShare = canShare;
        }

        /// <summary>Creates a new-signup user: active, non-admin and allowed to share, by default.</summary>
        public User(
            int userId,
            string name,
            string email,
            string password)
        {
            UserId = userId;
            Name = name;
            Email = email;
            Password = password;

            IsActive = true;
            IsAdmin = false;
            CanShare = true;
        }


        // =========================
        // Properties
        // =========================

        public int UserId
        {
            get => userId;
            set => userId = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Email
        {
            get => email;
            set => email = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }

        public bool IsAdmin
        {
            get => isAdmin;
            set => isAdmin = value;
        }

        public bool CanShare
        {
            get => canShare;
            set => canShare = value;
        }


        // =====================================================
        // Authentication
        // =====================================================

        /// <summary>
        /// Registers a new user. Throws a plain <see cref="Exception"/> if the email is
        /// already taken. Forces the new account to IsActive=true, IsAdmin=false,
        /// CanShare=true regardless of what was passed in. Returns the new user's id.
        /// </summary>
        public int Register(User user)
        {
            DBUserServices db = new DBUserServices();

            User existingUser = db.ReadUserByEmail(user.Email);

            if (existingUser != null)
            {
                throw new Exception(
                    "A user with this email already exists.");
            }

            user.IsActive = true;
            user.IsAdmin = false;
            user.CanShare = true;

            return db.InsertUser(user);
        }


        /// <summary>
        /// Verifies email/password and, on success, records a login event via
        /// <see cref="DAL.DBUserServices.InsertUserLogin"/>. Throws a plain <see cref="Exception"/>
        /// for an unknown email or wrong password, and <see cref="UnauthorizedAccessException"/>
        /// if the account has been deactivated (blocked) by an admin.
        /// </summary>
        public User Login(
    string email,
    string password)
        {
            DBUserServices db =
                new DBUserServices();

            User user =
                db.ReadUserByEmail(email);

            if (user == null)
            {
                throw new Exception(
                    "Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException(
                    "This user is blocked.");
            }

            if (user.Password != password)
            {
                throw new Exception(
                    "Invalid email or password.");
            }

            // Login succeeded
            db.InsertUserLogin(user.UserId);

            return user;
        }


        // =====================================================
        // Read
        // =====================================================

        /// <summary>Returns the user matching <paramref name="userId"/>, or null if none exists.</summary>
        public User ReadById(int userId)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadUserById(userId);
        }


        /// <summary>Returns the user matching <paramref name="name"/>, or null if none exists.</summary>
        public User ReadByName(string name)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadUserByName(name);
        }


        /// <summary>Returns every user in the database (admin user-management list).</summary>
        public List<User> ReadAllUsers()
        {
            DBUserServices db = new DBUserServices();

            return db.ReadAllUsers();
        }


        // =====================================================
        // User Profile
        // =====================================================

        /// <summary>
        /// Updates the profile (name/email) of <paramref name="userTargetId"/>. Callable by the
        /// user themselves or by an admin; anyone else gets <see cref="UnauthorizedAccessException"/>.
        /// Throws <see cref="ArgumentException"/> if name or email is blank.
        /// </summary>
        public int UpdateProfile(
            int userTargetId,
            User userDetails)
        {
            if (this.UserId != userTargetId && !this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "A user can only update his own profile.");
            }

            if (string.IsNullOrWhiteSpace(userDetails.Name))
            {
                throw new ArgumentException(
                    "Name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(userDetails.Email))
            {
                throw new ArgumentException(
                    "Email cannot be empty.");
            }

            DBUserServices db = new DBUserServices();

            return db.UpdateUser(userTargetId, userDetails);
        }


        // =====================================================
        // Password
        // =====================================================

        /// <summary>
        /// Changes <paramref name="userTargetId"/>'s password after verifying
        /// <paramref name="currentPassword"/> matches. Can only be called by the user themselves
        /// (no admin override) - throws <see cref="UnauthorizedAccessException"/> otherwise, or if
        /// the current password is wrong. Throws <see cref="ArgumentException"/> if the new password is blank.
        /// </summary>
        public int SetPassword(
            int userTargetId,
            User userDetails,
            string currentPassword,
            string newPassword)
        {
            if (this.UserId != userTargetId)
            {
                throw new UnauthorizedAccessException(
                    "A user can only change his own password.");
            }

            if (this.Password != currentPassword)
            {
                throw new UnauthorizedAccessException(
                    "Current password is incorrect.");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException(
                    "New password cannot be empty.");
            }

            userDetails.Password = newPassword;

            DBUserServices db = new DBUserServices();

            return db.UpdateUser(userTargetId, userDetails);
        }


        // =====================================================
        // Admin - User Status
        // =====================================================

        /// <summary>Activates/deactivates (blocks) <paramref name="userTargetId"/>. Admin-only; throws <see cref="UnauthorizedAccessException"/> otherwise.</summary>
        public int SetUserActive(
            int userTargetId,
            User userDetails)
        {
            if (!this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only admins can change user status.");
            }

            DBUserServices db = new DBUserServices();

            return db.UpdateUser(userTargetId, userDetails);
        }


        /// <summary>Grants/revokes <paramref name="userTargetId"/>'s permission to share reviews. Admin-only; throws <see cref="UnauthorizedAccessException"/> otherwise.</summary>
        public int SetCanShare(
            int userTargetId,
            User userDetails)
        {
            if (!this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only admins can change sharing permissions.");
            }

            DBUserServices db = new DBUserServices();

            return db.UpdateUser(userTargetId, userDetails);
        }


        /// <summary>Grants/revokes admin rights for <paramref name="userTargetId"/>. Admin-only; throws <see cref="UnauthorizedAccessException"/> otherwise.</summary>
        public int SetAdmin(
            int userTargetId,
            User userDetails)
        {
            if (!this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only admins can change admin permissions.");
            }

            DBUserServices db = new DBUserServices();

            return db.UpdateUser(userTargetId, userDetails);
        }


        // =====================================================
        // User Preferences
        // =====================================================

        /// <summary>Returns the regions <paramref name="userId"/> has marked as a travel preference.</summary>
        public List<Region> ReadPreferredRegions(int userId)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadPreferredRegions(userId);
        }


        /// <summary>Replaces <paramref name="userId"/>'s preferred-region list with <paramref name="regionIds"/>.</summary>
        public void UpdatePreferredRegions(
            int userId,
            List<int> regionIds)
        {
            DBUserServices db = new DBUserServices();

            db.UpdatePreferredRegions(userId, regionIds);
        }


        /// <summary>Returns the languages (and proficiency levels) <paramref name="userId"/> has recorded.</summary>
        public List<UserLanguages> ReadUserLanguages(int userId)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadUserLanguages(userId);
        }


        /// <summary>Replaces <paramref name="userId"/>'s recorded languages with <paramref name="languages"/>.</summary>
        public void UpdateUserLanguages(
            int userId,
            List<UserLanguages> languages)
        {
            DBUserServices db = new DBUserServices();

            db.UpdateUserLanguages(userId, languages);
        }


        // =====================================================
        // Wanted Countries
        // =====================================================

        /// <summary>Returns the countries <paramref name="userId"/> has added to their wishlist ("wanted" countries).</summary>
        public List<Country> ReadWantedCountries(int userId)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadWantedCountries(userId);
        }


        /// <summary>
        /// Adds <paramref name="countryId"/> to this user's wishlist. UserWantedCountries has a
        /// composite (UserId, CountryId) primary key, so the calling controller must catch the
        /// SqlException (2627/2601) thrown on a duplicate insert and turn it into a 409 Conflict.
        /// </summary>
        public int AddWantedCountry(int countryId)
        {
            DBUserServices db = new DBUserServices();

            return db.AddWantedCountry(this.UserId, countryId);
        }


        /// <summary>Removes <paramref name="countryId"/> from this user's wishlist.</summary>
        public int RemoveWantedCountry(int countryId)
        {
            DBUserServices db = new DBUserServices();

            return db.RemoveWantedCountry(this.UserId, countryId);
        }




        /// <summary>Returns dashboard counters (daily logins, imported/saved countries, shared reviews). Admin-only; throws <see cref="UnauthorizedAccessException"/> otherwise.</summary>
        public AdminStatistics ReadStatistics()
        {
            if (!this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only admins can view statistics.");
            }

            DBUserServices db =
                new DBUserServices();

            return db.ReadStatistics();
        }
    }
}