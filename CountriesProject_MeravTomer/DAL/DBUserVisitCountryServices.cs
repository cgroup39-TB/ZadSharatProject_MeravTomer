using ServerSideCountriesProject_MeravTomer.BL;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

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


        public List<UserVisitedCountry> ReadAllVisits()
        {
            List<UserVisitedCountry> visits = new List<BL.UserVisitedCountry>();
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

            cmd = CreateCommandWithStoredProcedureGeneral("spReadAllVisits", con, null);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    UserVisitedCountry visit = new UserVisitedCountry();

                    visit.UserId = Convert.ToInt32(dataReader["UserId"]);
                    visit.CountryId = Convert.ToInt32(dataReader["CountryId"]);
                    visit.Rating = Convert.ToInt32(dataReader["Rating"]);
                    visit.ReviewText = dataReader["Rating"].ToString();
                    visit.IsShared = Convert.ToBoolean(dataReader["IsShared"]);

                  
                    visits.Add(visit);
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


        

        public List<UserVisitedCountry> ReadVisitsByUser(int userId)
        {
            List<UserVisitedCountry> visits = new List<UserVisitedCountry>();
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@UserId", userId);


            cmd = CreateCommandWithStoredProcedureGeneral("spReadVisitsByUser", con, paramDic);



            try
            {

                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {

                    UserVisitedCountry visit = new UserVisitedCountry();

                    visit.UserId = Convert.ToInt32(dataReader["UserId"]);
                    visit.CountryId = Convert.ToInt32(dataReader["CountryId"]);
                    visit.Rating = Convert.ToInt32(dataReader["Rating"]);
                    visit.ReviewText = dataReader["Rating"].ToString();
                    visit.IsShared = Convert.ToBoolean(dataReader["IsShared"]);

                    visits.Add(visit);                
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

        public List<UserVisitedCountry> ReadVisitsByCountry(int countryId)
        {
            List<UserVisitedCountry> visits = new List<UserVisitedCountry>();
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral("spReadVisitsByCountry", con, paramDic);

            try {

                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dataReader.Read())
                {
                    UserVisitedCountry visit = new UserVisitedCountry();

                    visit.UserId = Convert.ToInt32(dataReader["UserId"]);
                    visit.CountryId = Convert.ToInt32(dataReader["CountryId"]);
                    visit.Rating = Convert.ToInt32(dataReader["Rating"]);
                    visit.ReviewText = dataReader["Rating"].ToString();
                    visit.IsShared = Convert.ToBoolean(dataReader["IsShared"]);
                    visits.Add(visit);
                }

                return visits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                    con.Close();
            }

        }

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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@UserId", visit.UserId);
            paramDic.Add("@CountryId", visit.CountryId);
            paramDic.Add("@Rating", visit.Rating);
            paramDic.Add("@ReviewText", visit.ReviewText);
            paramDic.Add("@IsShared", visit.IsShared);

            cmd = CreateCommandWithStoredProcedureGeneral("spUpdateVisit", con, paramDic);
            try
            {
                int numEffected = cmd.ExecuteNonQuery();
                if (numEffected > 0)
                {
                    return numEffected;
                }
                else {
                    throw new Exception("No rows were updated."); }

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


        public bool DeleteVisit(int userId, int countryId)
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@UserId", userId);
            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral("spDeleteVisit", con, paramDic);

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@UserId", visit.UserId);
            paramDic.Add("@CountryId", visit.CountryId);
            paramDic.Add("@Rating", visit.Rating);
            paramDic.Add("@ReviewText", visit.ReviewText);
            paramDic.Add("@IsShared", visit.IsShared);

            cmd = CreateCommandWithStoredProcedureGeneral("spInsertVisit", con, paramDic);


            try
            {
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
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



        //public UserVisitedCountry ReadVisit(int userId, int countryId)
        //{
        //    using (SqlConnection con = connect("myProjDB"))
        //    {
        //        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        //        paramDic.Add("@UserId", userId);
        //        paramDic.Add("@CountryId", countryId);

        //        SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("spReadVisit", con, paramDic);

        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                int rating = reader.GetInt32(reader.GetOrdinal("Rating"));
        //                string reviewText = reader.GetString(reader.GetOrdinal("ReviewText"));
        //                bool isShared = reader.GetBoolean(reader.GetOrdinal("IsShared"));

        //                return new UserVisitedCountry(userId, countryId, rating, reviewText, isShared);
        //            }
        //        }
        //    }

        //    return null; // Return null if no visit is found
        //}


       

    }
}
