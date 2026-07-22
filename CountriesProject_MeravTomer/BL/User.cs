namespace ServerSideCountriesProject_MeravTomer.BL
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

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

        public User(int userId, string name, string email, string password)
        {
            UserId = userId;
            Name = name;
            Email = email;
            Password = password;
            IsActive = true;
            IsAdmin = false;
            CanShare = true;
        }

        public int UserId { get => userId; set => userId = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public bool IsActive { get => isActive; private set => isActive = value; }
        public bool IsAdmin { get => isAdmin; private set => isAdmin = value; }
        public bool CanShare { get => canShare; private set => canShare = value; }



        public void SetCanShare(User user, bool valueShare)
        {
            if (!this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only admin users can change the CanShare property.");
            }

            user.CanShare = valueShare;
        }
        public void SetIsActive(User user, bool valueActive)
        {
            if (!this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only admin users can change the IsActive property.");
            }

            user.IsActive = valueActive; //Use of the private property set (outside we can only use the user method SetIsActive)
        }

        public void MakeAdmin(User user, bool valueAdmin)
        {
            if (!this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only admin users can make other users Admins.");
            }

            user.IsAdmin = valueAdmin; //Use of the private property set (outside we can only use the user method SetIsActive)
        }

        // Authentication
        //public User Register(User user)
        //{
        //}
        //public User Login(string email, string password)
        //{ }
        //public void SetPassword(int userId, string currentPassword, string newPassword)
        //{ }

        //// User Profile
        //public User ReadById(int userId) { }
        //public User ReadByName(string name) { }
        //public void UpdateProfile(User user) { }
        // User Profile
        public User (int userId) { }
        //public User ReadByName(string name) { }
        //public int UpdateProfile(User user) { }

        //// User Preferences
        //public List<Language> ReadUserLanguages(int userId) { }
        //public void UpdateUserLanguages(int userId, List<int> languageIds) { }

        //public List<Region> ReadPreferredRegions(int userId) { }
        //public void UpdatePreferredRegions(int userId, List<int> regionIds) { }

        //// User Status (Admin)
        //public void SetUserActive(int actingUserId, int targetUserId, bool isActive);
        //public void SetCanShare(int actingUserId, int targetUserId, bool canShare);
        //public void SetAdmin(int actingUserId, int targetUserId, bool isAdmin);
        //public List<User> ReadAllUsers() { }
        //public List<User> ReadStatistics() { }
    }

}

