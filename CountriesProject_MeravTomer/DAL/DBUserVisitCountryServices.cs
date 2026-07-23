using ServerSideCountriesProject_MeravTomer.BL;
using System.Data;
using System.Data.SqlClient;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBUserVisitCountryServices
    {
        public DBUserVisitCountryServices()
        {
        }


        //--------------------------------------------------------------------------------------------------
        // Create DB connection
        //--------------------------------------------------------------------------------------------------
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
                        param.Value);
                }
            }

            return cmd;
        }


        //--------------------------------------------------------------------------------------------------
        // Read all visited countries of a specific User
        //--------------------------------------------------------------------------------------------------
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
                "spReadVisitsByUser",
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
                "spReadVisitsByCountry",
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
                "spReadSharedVisitsByCountry",
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
        // Read shared reviews of a specific User
        //--------------------------------------------------------------------------------------------------
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
                "spReadSharedVisitsByUser",
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
        // Insert Visit
        //--------------------------------------------------------------------------------------------------
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
                "spInsertVisit",
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
                "spUpdateVisit",
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
                "spDeleteVisit",
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