using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class User
    {
        private int userId;
        private string name;
        private string email;
        private string password;
        private bool isActive;
        private bool isAdmin;
        private bool canShare;

        public User()
        {
        }

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


        public User Login(string email, string password)
        {
            DBUserServices db = new DBUserServices();

            User user = db.ReadUserByEmail(email);

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

            return user;
        }


        // =====================================================
        // Read
        // =====================================================

        public User ReadById(int userId)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadUserById(userId);
        }


        public User ReadByName(string name)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadUserByName(name);
        }


        public List<User> ReadAllUsers()
        {
            DBUserServices db = new DBUserServices();

            return db.ReadAllUsers();
        }


        // =====================================================
        // User Profile
        // =====================================================

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

        public List<Region> ReadPreferredRegions(int userId)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadPreferredRegions(userId);
        }


        public void UpdatePreferredRegions(
            int userId,
            List<int> regionIds)
        {
            DBUserServices db = new DBUserServices();

            db.UpdatePreferredRegions(userId, regionIds);
        }


        public List<UserLanguages> ReadUserLanguages(int userId)
        {
            DBUserServices db = new DBUserServices();

            return db.ReadUserLanguages(userId);
        }


        public void UpdateUserLanguages(
            int userId,
            List<UserLanguages> languages)
        {
            DBUserServices db = new DBUserServices();

            db.UpdateUserLanguages(userId, languages);
        }
    }
}