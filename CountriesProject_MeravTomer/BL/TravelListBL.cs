using ServerSideCountriesProject_MeravTomer.DAL;
using ServerSideCountriesProject_MeravTomer.Models;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class TravelListBL
    {
        private readonly DBVisitedCountriesServices dbVisitedCountriesServices = new DBVisitedCountriesServices();
        private readonly DBUserServices dbUserServices = new DBUserServices();
        private readonly DBCountryServices dbCountryServices = new DBCountryServices();

        // Returns null when the user itself doesn't exist, and an empty list when the user
        // simply hasn't visited any countries yet - the two are not the same thing.
        public List<UserVisitedCountry> GetVisitedCountries(int userId)
        {
            return dbUserServices.GetUserById(userId) == null ? null : dbVisitedCountriesServices.GetVisitedCountries(userId);
        }

        public UserVisitedCountry AddVisitedCountry(int userId, int countryId)
        {
            if (dbUserServices.GetUserById(userId) == null)
                return null;

            if (dbCountryServices.GetCountryById(countryId) == null)
                throw new ArgumentException("Country not found.");

            if (dbVisitedCountriesServices.IsCountryVisited(userId, countryId))
                throw new ArgumentException("This country is already in the visited list.");

            dbVisitedCountriesServices.InsertVisitedCountry(userId, countryId);

            return new UserVisitedCountry { UserId = userId, CountryId = countryId, IsShared = false };
        }

        public bool RemoveVisitedCountry(int userId, int countryId)
        {
            if (!dbVisitedCountriesServices.IsCountryVisited(userId, countryId))
                return false;

            dbVisitedCountriesServices.DeleteVisitedCountry(userId, countryId);
            return true;
        }

        public bool UpdateRating(int userId, int countryId, int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            if (!dbVisitedCountriesServices.IsCountryVisited(userId, countryId))
                return false;

            dbVisitedCountriesServices.UpdateRating(userId, countryId, rating);
            return true;
        }

        public bool UpdateReview(int userId, int countryId, string reviewText)
        {
            if (!dbVisitedCountriesServices.IsCountryVisited(userId, countryId))
                return false;

            dbVisitedCountriesServices.UpdateReview(userId, countryId, reviewText);
            return true;
        }

        public bool SetReviewShared(int userId, int countryId, bool isShared)
        {
            if (!dbVisitedCountriesServices.IsCountryVisited(userId, countryId))
                return false;

            dbVisitedCountriesServices.UpdateIsShared(userId, countryId, isShared);
            return true;
        }
    }
}
