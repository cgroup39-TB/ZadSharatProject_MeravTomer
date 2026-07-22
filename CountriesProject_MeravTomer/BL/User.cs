namespace ServerSideCountriesProject_MeravTomer.BL
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    public class User
    {
        private int id;
        private string name;
        private string email;
        private string password;
        private bool isActive;
        private bool isAdmin;
        private bool canShare;

        public User()
        {

        }

        public User(int id, string name, string email, string password, bool active)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            IsActive = active;
            IsAdmin = false;
            CanShare = true;



        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }
        public bool CanShare { get => canShare; set => canShare = value; }




        public void SetActive(bool active)
        {
            IsActive = active;
        }

        public void SetAdmin(bool admin)
        {
            if (this.IsAdmin)
            {
                IsAdmin = admin;
            }
            else
            {
                throw new UnauthorizedAccessException("Only admin users can change the IsAdmin property.");
            }
        }

        public void SetCanShare(User user)
        {
            if (this.IsAdmin)
            {
                CanShare = user.CanShare;
            }
            else
            {
                throw new UnauthorizedAccessException("Only admin users can change the CanShare property.");
            }

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