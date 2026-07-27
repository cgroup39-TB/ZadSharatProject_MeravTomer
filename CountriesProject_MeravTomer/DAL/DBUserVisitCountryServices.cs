using ServerSideCountriesProject_MeravTomer.BL;
using System.Data;
using System.Data.SqlClient;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    /// <summary>
    /// Data access layer for user-visited-country records: visits, ratings, written reviews,
    /// and sharing of reviews between users. The primary key of a visit is the (UserId, CountryId) pair.
    /// </summary>
    public class DBUserVisitCountryServices
    {
        /// <summary>
        /// Creates a new instance of the service. Holds no state; a fresh DB connection is opened per call.
        /// </summary>
        public DBUserVisitCountryServices()
        {
        }


        //--------------------------------------------------------------------------------------------------
        // Create DB connection
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Opens and returns a new <see cref="SqlConnection"/> using the named connection string
        /// read from appsettings.json. The caller is responsible for closing the connection.
        /// </summary>
        public SqlConnection connect(String conString)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string cStr =
                configuration.GetConnectionString(conString);

            SqlConnection connectionToDb =
                new SqlConnection(cStr);

            connectionToDb.Open();

            return connectionToDb;
        }


        //---------------------------------------------------------------------------------
        // Create SqlCommand with Stored Procedure
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureGeneral(
            String spName,
            SqlConnection con,
            Dictionary<string, object> paramDic)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            cmd.CommandText = spName;
            cmd.CommandTimeout = 10;
            cmd.CommandType = CommandType.StoredProcedure;

            if (paramDic != null)
            {
                foreach (KeyValuePair<string, object> param in paramDic)
                {
                    cmd.Parameters.AddWithValue(
                        param.Key,
                        param.Value ?? DBNull.Value);
                }
            }

            return cmd;
        }


        //--------------------------------------------------------------------------------------------------
        // Read all visited countries of a specific User
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns every country the given user has marked as visited (with rating, review text, and
        /// sharing flag). Each result's <see cref="Country"/> is loaded with a separate follow-up query per row.
        /// </summary>
        public List<UserVisitedCountry> ReadVisitsByUser(int userId)
        {
            List<UserVisitedCountry> visits =
                new List<UserVisitedCountry>();

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@UserId", userId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadVisitsByUser_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    UserVisitedCountry visit =
                        new UserVisitedCountry();

                    visit.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    Country country = new Country();

                    country.CountryId =
                        Convert.ToInt32(dataReader["CountryId"]);

                    visit.Country = country;

                    visit.Rating =
                        Convert.ToInt32(dataReader["Rating"]);

                    visit.ReviewText =
                        dataReader["ReviewText"].ToString();

                    visit.IsShared =
                        Convert.ToBoolean(dataReader["IsShared"]);

                    visits.Add(visit);
                }

                dataReader.Close();

                // Load full Country objects
                DBCountryServices countryDB =
                    new DBCountryServices();

                foreach (UserVisitedCountry visit in visits)
                {
                    visit.Country =
                        countryDB.ReadCountryById(
                            visit.Country.CountryId);
                }

                return visits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Read visits of a specific Country
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns every visit record (by any user) for the given country, with each result's
        /// <see cref="Country"/> loaded via a separate follow-up query per row.
        /// </summary>
        public List<UserVisitedCountry> ReadVisitsByCountry(
            int countryId)
        {
            List<UserVisitedCountry> visits =
                new List<UserVisitedCountry>();

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadVisitsByCountry_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    UserVisitedCountry visit =
                        new UserVisitedCountry();

                    visit.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    Country country = new Country();

                    country.CountryId =
                        Convert.ToInt32(dataReader["CountryId"]);

                    visit.Country = country;

                    visit.Rating =
                        Convert.ToInt32(dataReader["Rating"]);

                    visit.ReviewText =
                        dataReader["ReviewText"].ToString();

                    visit.IsShared =
                        Convert.ToBoolean(dataReader["IsShared"]);

                    visits.Add(visit);
                }

                dataReader.Close();

                DBCountryServices countryDB =
                    new DBCountryServices();

                foreach (UserVisitedCountry visit in visits)
                {
                    visit.Country =
                        countryDB.ReadCountryById(
                            visit.Country.CountryId);
                }

                return visits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Read shared reviews of a specific Country
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns only the visits for the given country that have been marked as shared
        /// (IsShared = true), including the review's creation date and the reviewing user's name.
        /// </summary>
        public List<UserVisitedCountry> ReadSharedVisitsByCountry(
            int countryId)
        {
            List<UserVisitedCountry> visits =
                new List<UserVisitedCountry>();

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadSharedVisitsByCountry_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    UserVisitedCountry visit =
                        new UserVisitedCountry();

                    visit.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    Country country = new Country();

                    country.CountryId =
                        Convert.ToInt32(dataReader["CountryId"]);

                    visit.Country = country;

                    visit.Rating =
                        Convert.ToInt32(dataReader["Rating"]);

                    visit.ReviewText =
                        dataReader["ReviewText"].ToString();

                    visit.IsShared =
                        Convert.ToBoolean(dataReader["IsShared"]);

                    visit.CreatedAt =
                        Convert.ToDateTime(dataReader["CreatedAt"]);

                    visit.UserName =
                        dataReader["UserName"].ToString();

                    visits.Add(visit);
                }

                dataReader.Close();

                DBCountryServices countryDB =
                    new DBCountryServices();

                foreach (UserVisitedCountry visit in visits)
                {
                    visit.Country =
                        countryDB.ReadCountryById(
                            visit.Country.CountryId);
                }

                return visits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Read shared reviews of a specific User
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns only the given user's visits that have been marked as shared (IsShared = true),
        /// including the review's creation date and the user's name.
        /// </summary>
        public List<UserVisitedCountry> ReadSharedVisitsByUser(
            int userId)
        {
            List<UserVisitedCountry> visits =
                new List<UserVisitedCountry>();

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@UserId", userId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadSharedVisitsByUser_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    UserVisitedCountry visit =
                        new UserVisitedCountry();

                    visit.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    Country country = new Country();

                    country.CountryId =
                        Convert.ToInt32(dataReader["CountryId"]);

                    visit.Country = country;

                    visit.Rating =
                        Convert.ToInt32(dataReader["Rating"]);

                    visit.ReviewText =
                        dataReader["ReviewText"].ToString();

                    visit.IsShared =
                        Convert.ToBoolean(dataReader["IsShared"]);

                    visit.CreatedAt =
                        Convert.ToDateTime(dataReader["CreatedAt"]);

                    visit.UserName =
                        dataReader["UserName"].ToString();

                    visits.Add(visit);
                }

                dataReader.Close();

                DBCountryServices countryDB =
                    new DBCountryServices();

                foreach (UserVisitedCountry visit in visits)
                {
                    visit.Country =
                        countryDB.ReadCountryById(
                            visit.Country.CountryId);
                }

                return visits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Read all shared reviews (every country, every user)
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns every shared review (IsShared = true) across all users and countries,
        /// including creation date and reviewer name.
        /// </summary>
        public List<UserVisitedCountry> ReadAllSharedVisits()
        {
            List<UserVisitedCountry> visits =
                new List<UserVisitedCountry>();

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadAllSharedVisits_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    UserVisitedCountry visit =
                        new UserVisitedCountry();

                    visit.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    Country country = new Country();

                    country.CountryId =
                        Convert.ToInt32(dataReader["CountryId"]);

                    visit.Country = country;

                    visit.Rating =
                        Convert.ToInt32(dataReader["Rating"]);

                    visit.ReviewText =
                        dataReader["ReviewText"].ToString();

                    visit.IsShared =
                        Convert.ToBoolean(dataReader["IsShared"]);

                    visit.CreatedAt =
                        Convert.ToDateTime(dataReader["CreatedAt"]);

                    visit.UserName =
                        dataReader["UserName"].ToString();

                    visits.Add(visit);
                }

                dataReader.Close();

                DBCountryServices countryDB =
                    new DBCountryServices();

                foreach (UserVisitedCountry visit in visits)
                {
                    visit.Country =
                        countryDB.ReadCountryById(
                            visit.Country.CountryId);
                }

                return visits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Insert Visit
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts a new visit record for the (UserId, CountryId) pair carried by <paramref name="visit"/>.
        /// Because that pair is the table's composite primary key, inserting a duplicate visit throws a
        /// SqlException (error 2627/2601) which propagates out of this method to be handled by the caller
        /// (the controller maps it to HTTP 409 Conflict).
        /// </summary>
        /// <returns>The number of rows affected, as reported by the stored procedure.</returns>
        public int InsertVisit(UserVisitedCountry visit)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@UserId", visit.UserId);

            paramDic.Add(
                "@CountryId",
                visit.Country.CountryId);

            paramDic.Add("@Rating", visit.Rating);
            paramDic.Add("@ReviewText", visit.ReviewText);
            paramDic.Add("@IsShared", visit.IsShared);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertVisit_3MD_TB",
                con,
                paramDic);

            try
            {
                int numEffected =
                    cmd.ExecuteNonQuery();

                return numEffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Update Visit
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the rating, review text, and sharing flag of the existing visit identified by
        /// <paramref name="visit"/>'s UserId/CountryId pair.
        /// </summary>
        /// <returns>The number of rows affected; 0 if no matching visit was found.</returns>
        public int UpdateVisit(UserVisitedCountry visit)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@UserId", visit.UserId);

            paramDic.Add(
                "@CountryId",
                visit.Country.CountryId);

            paramDic.Add("@Rating", visit.Rating);
            paramDic.Add("@ReviewText", visit.ReviewText);
            paramDic.Add("@IsShared", visit.IsShared);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spUpdateVisit_3MD_TB",
                con,
                paramDic);

            try
            {
                int numEffected =
                    cmd.ExecuteNonQuery();

                return numEffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Delete Visit
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the visit record for the given (userId, countryId) pair.
        /// </summary>
        /// <returns>True if a row was deleted; false if no matching visit existed.</returns>
        public bool DeleteVisit(
            int userId,
            int countryId)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@UserId", userId);
            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spDeleteVisit_3MD_TB",
                con,
                paramDic);

            try
            {
                int rowsAffected =
                    cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
    }
}