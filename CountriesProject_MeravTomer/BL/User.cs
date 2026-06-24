namespace CountriesProject_MeravTomer.BL
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
        private bool active;

        public User()
        {

        }

        public User(int id, string name, string email, string password, bool active)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Active = active;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public bool Active { get => active; set => active = value; }

    }
}