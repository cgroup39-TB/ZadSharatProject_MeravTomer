namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>
    /// Aggregated counters shown on the admin dashboard (logins, imported/saved
    /// countries, shared reviews). Populated by <see cref="DAL.DBUserServices.ReadStatistics"/>.
    /// </summary>
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