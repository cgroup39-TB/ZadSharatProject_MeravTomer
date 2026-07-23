namespace ServerSideCountriesProject_MeravTomer.Models
{
    public class UserVisitedCountry
    {
        public int UserId { get; set; }
        public int CountryId { get; set; }
        public int? Rating { get; set; }
        public string ReviewText { get; set; }
        public bool IsShared { get; set; }
        public Country Country { get; set; }

        public UserVisitedCountry()
        {
        }
    }
}
