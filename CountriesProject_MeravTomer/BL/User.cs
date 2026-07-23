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
        private List<Region> prefferedRegions;
        private List<UserVisitedCountry> visitedCountries;
        private List<UserLanguages> prefferedLanguages;//

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

        //Authentication
        public User Register(User user)//SQL-InsertUser
        {

        }
        public User Login(string email, string password)//SQL-ReadByEmail
        { }
        public void SetPassword(int userId, string currentPassword, string newPassword)//SQL-Update User
        { }
        
        //User Profile
        public User(int userId) { }
        public User ReadById(int userId) { }//?
        public User ReadByName(string name) { }//ReadByName
        public int UpdateProfile(User user) { }// SQL-Update user

        // User Preferences
        public List<Language> ReadUserLanguages(int userId) { } //Need Create SP -ReadUserLang
        public void UpdateUserLanguages(int userId, List<int> languageIds) { } //Need Create SP -UpdateUserLang

        public List<Region> ReadPreferredRegions(int userId) { }//Need Create SP -ReadUserRegions
        public void UpdatePreferredRegions(int userId, List<int> regionIds) { }//Need Create SP -UpdateUserRegions

        // User Status (Admin)
        public void SetUserActive(int actingUserId, int targetUserId, bool isActive);//SQL-Update User
        public void SetCanShare(int actingUserId, int targetUserId, bool canShare);//SQL-Update User
        public void SetAdmin(int actingUserId, int targetUserId, bool isAdmin);//SQL-Update User
        public List<User> ReadAllUsers() { }//SQL-ReadAllUsers
        public List<User> ReadStatistics() { }//Need Create -ReadStatistics
    }

}

