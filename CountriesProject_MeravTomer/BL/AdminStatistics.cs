namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class AdminStatistics
    {
        public int DailyLogins { get; set; }
        public int ImportedCountries { get; set; }
        public int SavedCountries { get; set; }
        public int SharedReviews { get; set; }

        public AdminStatistics()
        {
        }
    }
}