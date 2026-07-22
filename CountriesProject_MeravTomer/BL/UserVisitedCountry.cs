using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class UserVisitedCountry
    {
        private int userId;
        private int countryId;
        private int rating;
        private string reviewText;
        private bool isShared;

        public int UserId { get => userId; set => userId = value; }
        public int CountryId { get => countryId; set => countryId = value; }
        public int Rating { get => rating; set => rating = value; }
        public string ReviewText { get => reviewText; set => reviewText = value; }
        public bool IsShared { get => isShared; set => isShared = value; }


        public UserVisitedCountry(int userId, int countryId, int rating, string reviewText, bool isShared)
        {
            UserId = userId;
            CountryId = countryId;
            Rating = rating;
            ReviewText = reviewText;
            IsShared = isShared;
        }

        public UserVisitedCountry() { }



        public UserVisitedCountry Insert()
        {
            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return this;
        }

        public bool Update()
        {
            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return db.UpdateVisit(this);
        }

        public bool Delete()
        {
            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return db.DeleteVisit(userId, countryId);
        }

        public static List<UserVisitedCountry> ReadByUser(int userId)
        {
            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return db.ReadVisitsByUser(userId);
        }
      
        public static List<UserVisitedCountry> ReadAll()
        {
            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return db.ReadAllVisits();
        }

        public static List<UserVisitedCountry> ReadByCountry(int countryId)
        {
            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return db.ReadVisitsByCountry(countryId);
        }

        public List<UserVisitedCountry> ReadSharedByCountry(int countryId)
        {

            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return db.ReadSharedVisitsByCountry(countryId);
        }

        public List<UserVisitedCountry> ReadSharedByUser(int userId)
        {
            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return db.ReadSharedVisitsByUser(userId);
        }

        public bool Exists(int userId, int countryId)
        {
            DBUserVisitCountryServices db = new DBUserVisitCountryServices();
            return db.Exists(userId, countryId);
        }
    }


    
}
