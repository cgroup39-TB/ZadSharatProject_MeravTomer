using ServerSideCountriesProject_MeravTomer.BL;
using System.Data.SqlClient;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBUserVisitCountryServices
    {
        public DBUserVisitCountryServices() { }


        //--------------------------------------------------------------------------------------------------
        // This method creates a connection to the database according to the connectionString name in the web.config 
        //--------------------------------------------------------------------------------------------------
        public SqlConnection connect(String conString)
        {

            // read the connection string from the configuration file
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
            string cStr = configuration.GetConnectionString(conString);
            SqlConnection connectionToDb = new SqlConnection(cStr);
            connectionToDb.Open();
            return connectionToDb;
        }

        //---------------------------------------------------------------------------------
        // Create the SqlCommand
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureGeneral(String spName, SqlConnection con, Dictionary<string, object> paramDic)
        {

            SqlCommand cmd = new SqlCommand(); // create the command object

            cmd.Connection = con;              // assign the connection to the command object

            cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

            cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

            cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

            if (paramDic != null)
                foreach (KeyValuePair<string, object> param in paramDic)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);

                }


            return cmd;
        }


        public List<UserVisitedCountry> ReadVisitsByUser(int userId)
        {
            List<BL.UserVisitedCountry> visits = new List<BL.UserVisitedCountry>();

            using (SqlConnection con = connect("CountriesDB"))
            {
                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@UserId", userId);

                SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("spReadVisitsByUser", con, paramDic);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int countryId = reader.GetInt32(reader.GetOrdinal("CountryId"));
                        int rating = reader.GetInt32(reader.GetOrdinal("Rating"));
                        string reviewText = reader.GetString(reader.GetOrdinal("ReviewText"));
                        bool isShared = reader.GetBoolean(reader.GetOrdinal("IsShared"));

                        BL.UserVisitedCountry visit = new BL.UserVisitedCountry(userId, countryId, rating, reviewText, isShared);
                        visits.Add(visit);
                    }
                }
            }

            return visits;
        }



        public bool UpdateVisit(UserVisitedCountry visit)
        {
            using (SqlConnection con = connect("CountriesDB"))
            {
                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@UserId", visit.UserId);
                paramDic.Add("@CountryId", visit.CountryId);
                paramDic.Add("@Rating", visit.Rating);
                paramDic.Add("@ReviewText", visit.ReviewText);
                paramDic.Add("@IsShared", visit.IsShared);

                SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("spUpdateVisit", con, paramDic);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


        public bool DeleteVisit(int userId, int countryId)
        {
            using (SqlConnection con = connect("CountriesDB"))
            {
                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@UserId", userId);
                paramDic.Add("@CountryId", countryId);

                SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("spDeleteVisit", con, paramDic);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


        public bool InsertVisit(UserVisitedCountry visit)
        {
            using (SqlConnection con = connect("CountriesDB"))
            {
                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@UserId", visit.UserId);
                paramDic.Add("@CountryId", visit.CountryId);
                paramDic.Add("@Rating", visit.Rating);
                paramDic.Add("@ReviewText", visit.ReviewText);
                paramDic.Add("@IsShared", visit.IsShared);

                SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("spInsertVisit", con, paramDic);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }



        public UserVisitedCountry ReadVisit(int userId, int countryId)
        {
            using (SqlConnection con = connect("CountriesDB"))
            {
                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@UserId", userId);
                paramDic.Add("@CountryId", countryId);

                SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("spReadVisit", con, paramDic);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int rating = reader.GetInt32(reader.GetOrdinal("Rating"));
                        string reviewText = reader.GetString(reader.GetOrdinal("ReviewText"));
                        bool isShared = reader.GetBoolean(reader.GetOrdinal("IsShared"));

                        return new UserVisitedCountry(userId, countryId, rating, reviewText, isShared);
                    }
                }
            }

            return null; // Return null if no visit is found
        }

       

    }
}
