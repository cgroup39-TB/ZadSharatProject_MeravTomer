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
        public bool IsActive { get => isActive; set => isActive = value; }
        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }
        public bool CanShare { get => canShare; set => canShare = value; }



        public void SetCanShare(User user, bool valueShare)
        {
            if (!this.IsAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only admin users can change the CanShare property.");
            }

            user.CanShare = valueShare;
        }
        public void SetIsActive(User user)
        {
            if (this.IsAdmin)
            {
                IsActive = user.IsActive;
            }
            else
            {
                throw new UnauthorizedAccessException("Only admin users can change the IsActive property.");
            }
        }

    }
}