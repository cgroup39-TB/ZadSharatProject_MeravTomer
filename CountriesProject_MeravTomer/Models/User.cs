namespace ServerSideCountriesProject_MeravTomer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public bool CanShare { get; set; }

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
    }
}
