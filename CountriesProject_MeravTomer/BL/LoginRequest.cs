namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>
    /// Request body for the login endpoint. Kept as a minimal, purpose-built DTO
    /// (rather than reusing <see cref="User"/>) so a login POST only needs Email/Password.
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
