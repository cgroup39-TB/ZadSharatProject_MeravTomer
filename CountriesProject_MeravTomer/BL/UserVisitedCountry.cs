using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>
    /// Records that a user has visited a country, with an optional rating/review and a flag for
    /// whether the review is publicly shared. Has a composite (UserId, CountryId) primary key in
    /// the database - see <see cref="Insert"/>.
    /// </summary>
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

        /// <summary>Creates an empty instance (used by model binding/deserialization).</summary>
        public UserVisitedCountry()
        {
        }

        /// <summary>Creates a fully-populated visit record.</summary>
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
                // 0 means "not rated yet" (e.g. a country just marked as
                // visited, before the user writes a review).
                if (value != 0 && (value < 1 || value > 5))
                {
                    throw new ArgumentException(
                        "Rating must be 0 (unrated) or between 1 and 5.");
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

        // Populated only when reading shared reviews (joined from Users /
        // read from the DB) - not part of the writable visit data.
        public string UserName { get; set; }

        public DateTime? CreatedAt { get; set; }


        // =========================
        // CRUD
        // =========================

        /// <summary>
        /// Inserts this visit record. UserVisitedCountries has a composite (UserId, CountryId)
        /// primary key, so marking the same country as visited twice throws a SqlException
        /// (2627/2601) - the calling controller catches that and returns 409 Conflict.
        /// </summary>
        public UserVisitedCountry Insert()
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            db.InsertVisit(this);

            return this;
        }

        /// <summary>Updates the rating/review/share flag for this visit and returns the number of affected rows.</summary>
        public int Update()
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.UpdateVisit(this);
        }

        /// <summary>Deletes this visit record (by UserId + Country.CountryId). Returns true if a row was removed.</summary>
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

        /// <summary>Returns every country <paramref name="userId"/> has marked as visited (including unrated/unreviewed ones).</summary>
        public static List<UserVisitedCountry>
            ReadVisitedCountriesByUser(int userId)
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.ReadVisitsByUser(userId);
        }


        /// <summary>Returns every visit record for the given country, across all users.</summary>
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

        /// <summary>Returns only the visits for this country where IsShared is true (public reviews), including the reviewer's name.</summary>
        public static List<UserVisitedCountry>
            ReadSharedReviewsByCountry(int countryId)
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.ReadSharedVisitsByCountry(countryId);
        }


        /// <summary>Returns the shared (public) reviews written by this user.</summary>
        public static List<UserVisitedCountry>
            ReadSharedReviewsByUser(int userId)
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.ReadSharedVisitsByUser(userId);
        }


        /// <summary>Returns every shared (public) review across all users and countries.</summary>
        public static List<UserVisitedCountry>
            ReadAllSharedReviews()
        {
            DBUserVisitCountryServices db =
                new DBUserVisitCountryServices();

            return db.ReadAllSharedVisits();
        }
    }


}
