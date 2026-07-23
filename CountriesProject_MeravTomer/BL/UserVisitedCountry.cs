using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class UserVisitedCountry
    {
        private int userId;
        private Country country;
        private int rating;
        private string reviewText;
        private bool isShared;


        // =========================
        // Constructors
        // =========================

        public UserVisitedCountry()
        {
        }

        public UserVisitedCountry(
            int userId,
            Country country,
            int rating,
            string reviewText,
            bool isShared)
        {
            UserId = userId;
            Country = country;
            Rating = rating;
            ReviewText = reviewText;
            IsShared = isShared;
        }


        // =========================
        // Properties
        // =========================

        public int UserId
        {
            get => userId;
            set => userId = value;
        }

        public Country Country
        {
            get => country;
            set => country = value;
        }

        public int Rating
        {
            get => rating;
            set
            {
                if (value < 1 || value > 5)
                {
                    throw new ArgumentException(
                        "Rating must be between 1 and 5.");
                }

                rating = value;
            }
        }

        public string ReviewText
        {
            get => reviewText;
            set => reviewText = value;
        }

        public bool IsShared
        {
            get => isShared;
            set => isShared = value;
        }


        // =========================
        // CRUD
        // =========================

        public UserVisitedCountry Insert()
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            db.InsertVisit(this);

            return this;
        }

        public int Update()
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.UpdateVisit(this);
        }

        public bool Delete()
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.DeleteVisit(
                UserId,
                Country.CountryId
            );
        }


        // =========================
        // Read
        // =========================

        public static List<UserVisitedCountry>
            ReadVisitedCountriesByUser(int userId)
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.ReadVisitsByUser(userId);
        }


        public static List<UserVisitedCountry>
            ReadVisitsByCountry(int countryId)
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.ReadVisitsByCountry(countryId);
        }


        // =========================
        // Shared Reviews
        // =========================

        public static List<UserVisitedCountry>
            ReadSharedReviewsByCountry(int countryId)
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.ReadSharedVisitsByCountry(countryId);
        }


        public static List<UserVisitedCountry>
            ReadSharedReviewsByUser(int userId)
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.ReadSharedVisitsByUser(userId);
        }
    }


}
