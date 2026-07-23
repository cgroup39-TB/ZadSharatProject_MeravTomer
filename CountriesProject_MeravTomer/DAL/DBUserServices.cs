using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBUserServices
    {
        public DBUserServices()
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


        //==================================================================================================
        // USERS
        //==================================================================================================


        //--------------------------------------------------------------------------------------------------
        // Insert User
        //--------------------------------------------------------------------------------------------------
        public int InsertUser(User user)
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

            paramDic.Add("@Name", user.Name);
            paramDic.Add("@Email", user.Email);
            paramDic.Add("@Password", user.Password);
            paramDic.Add("@IsActive", user.IsActive);
            paramDic.Add("@IsAdmin", user.IsAdmin);
            paramDic.Add("@CanShare", user.CanShare);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertUser",
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
        // Read all Users
        //--------------------------------------------------------------------------------------------------
        public List<User> ReadAllUsers()
        {
            SqlConnection con;
            SqlCommand cmd;

            List<User> users =
                new List<User>();

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadAllUsers",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    User u = new User();

                    u.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    u.Name =
                        dataReader["Name"].ToString();

                    u.Email =
                        dataReader["Email"].ToString();

                    u.Password =
                        dataReader["Password"].ToString();

                    u.IsActive =
                        Convert.ToBoolean(dataReader["IsActive"]);

                    u.IsAdmin =
                        Convert.ToBoolean(dataReader["IsAdmin"]);

                    u.CanShare =
                        Convert.ToBoolean(dataReader["CanShare"]);

                    users.Add(u);
                }

                return users;
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
        // Read User by ID
        //--------------------------------------------------------------------------------------------------
        public User ReadUserById(int userId)
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

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadUserById",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    User u = new User();

                    u.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    u.Name =
                        dataReader["Name"].ToString();

                    u.Email =
                        dataReader["Email"].ToString();

                    u.Password =
                        dataReader["Password"].ToString();

                    u.IsActive =
                        Convert.ToBoolean(dataReader["IsActive"]);

                    u.IsAdmin =
                        Convert.ToBoolean(dataReader["IsAdmin"]);

                    u.CanShare =
                        Convert.ToBoolean(dataReader["CanShare"]);

                    return u;
                }

                return null;
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
        // Read User by Email
        // Used for Login / Register check
        //--------------------------------------------------------------------------------------------------
        public User ReadUserByEmail(string email)
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

            paramDic.Add("@Email", email);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadUserByEmail",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    User u = new User();

                    u.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    u.Name =
                        dataReader["Name"].ToString();

                    u.Email =
                        dataReader["Email"].ToString();

                    u.Password =
                        dataReader["Password"].ToString();

                    u.IsActive =
                        Convert.ToBoolean(dataReader["IsActive"]);

                    u.IsAdmin =
                        Convert.ToBoolean(dataReader["IsAdmin"]);

                    u.CanShare =
                        Convert.ToBoolean(dataReader["CanShare"]);

                    return u;
                }

                return null;
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
        // Read User by Name
        //--------------------------------------------------------------------------------------------------
        public User ReadUserByName(string name)
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

            paramDic.Add("@Name", name);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadUserByName",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    User u = new User();

                    u.UserId =
                        Convert.ToInt32(dataReader["UserId"]);

                    u.Name =
                        dataReader["Name"].ToString();

                    u.Email =
                        dataReader["Email"].ToString();

                    u.Password =
                        dataReader["Password"].ToString();

                    u.IsActive =
                        Convert.ToBoolean(dataReader["IsActive"]);

                    u.IsAdmin =
                        Convert.ToBoolean(dataReader["IsAdmin"]);

                    u.CanShare =
                        Convert.ToBoolean(dataReader["CanShare"]);

                    return u;
                }

                return null;
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
        // Update User
        // Used by BL methods after their validation
        //--------------------------------------------------------------------------------------------------
        public int UpdateUser(
            int userToUpdateId,
            User userDetails)
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

            paramDic.Add("@UserId", userToUpdateId);
            paramDic.Add("@Name", userDetails.Name);
            paramDic.Add("@Email", userDetails.Email);
            paramDic.Add("@Password", userDetails.Password);
            paramDic.Add("@IsActive", userDetails.IsActive);
            paramDic.Add("@IsAdmin", userDetails.IsAdmin);
            paramDic.Add("@CanShare", userDetails.CanShare);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spUpdateUser",
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


        //==================================================================================================
        // USER REGIONS
        //==================================================================================================


        //--------------------------------------------------------------------------------------------------
        // Read preferred Regions of User
        //--------------------------------------------------------------------------------------------------
        public List<Region> ReadPreferredRegions(int userId)
        {
            SqlConnection con;
            SqlCommand cmd;

            List<Region> regions =
                new List<Region>();

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
                "spReadUserRegions",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Region region = new Region();

                    region.RegionId =
                        Convert.ToInt32(dataReader["RegionId"]);

                    region.RegionName =
                        dataReader["RegionName"].ToString();

                    regions.Add(region);
                }

                return regions;
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
        // Replace User preferred Regions
        //--------------------------------------------------------------------------------------------------
        public void UpdatePreferredRegions(
            int userId,
            List<int> regionIds)
        {
            DeleteUserRegions(userId);

            if (regionIds == null ||
                regionIds.Count == 0)
            {
                return;
            }

            SqlConnection con;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                foreach (int regionId in regionIds)
                {
                    Dictionary<string, object> paramDic =
                        new Dictionary<string, object>();

                    paramDic.Add("@UserId", userId);
                    paramDic.Add("@RegionId", regionId);

                    SqlCommand cmd =
                        CreateCommandWithStoredProcedureGeneral(
                            "spInsertUserRegion",
                            con,
                            paramDic);

                    cmd.ExecuteNonQuery();
                }
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
        // Delete User Regions
        //--------------------------------------------------------------------------------------------------
        public int DeleteUserRegions(int userId)
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

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spDeleteUserRegions",
                con,
                paramDic);

            try
            {
                return cmd.ExecuteNonQuery();
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


        //==================================================================================================
        // USER LANGUAGES
        //==================================================================================================


        //--------------------------------------------------------------------------------------------------
        // Read User Languages
        // UserLanguages contains UserId + Language Object + LevelLanguage
        //--------------------------------------------------------------------------------------------------
        public List<UserLanguages> ReadUserLanguages(int userId)
        {
            SqlConnection con;
            SqlCommand cmd;

            List<UserLanguages> userLanguages =
                new List<UserLanguages>();

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
                "spReadUserLanguages",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Language language =
                        new Language(
                            Convert.ToInt32(
                                dataReader["LanguageId"]),
                            dataReader["LanguageName"].ToString()
                        );

                    int? levelLanguage = null;

                    if (dataReader["LevelLanguage"] != DBNull.Value)
                    {
                        levelLanguage =
                            Convert.ToInt32(
                                dataReader["LevelLanguage"]);
                    }

                    UserLanguages userLanguage =
                        new UserLanguages(
                            userId,
                            language,
                            levelLanguage
                        );

                    userLanguages.Add(userLanguage);
                }

                return userLanguages;
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
        // Replace User Languages
        //--------------------------------------------------------------------------------------------------
        public void UpdateUserLanguages(
            int userId,
            List<UserLanguages> languages)
        {
            DeleteUserLanguages(userId);

            if (languages == null ||
                languages.Count == 0)
            {
                return;
            }

            SqlConnection con;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                foreach (UserLanguages userLanguage in languages)
                {
                    Dictionary<string, object> paramDic =
                        new Dictionary<string, object>();

                    paramDic.Add("@UserId", userId);

                    paramDic.Add(
                        "@LanguageId",
                        userLanguage.Language.LanguageId);

                    paramDic.Add(
                        "@LevelLanguage",
                        userLanguage.LevelLanguage.HasValue
                            ? userLanguage.LevelLanguage.Value
                            : DBNull.Value);

                    SqlCommand cmd =
                        CreateCommandWithStoredProcedureGeneral(
                            "spInsertUserLanguage",
                            con,
                            paramDic);

                    cmd.ExecuteNonQuery();
                }
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
        // Delete User Languages
        //--------------------------------------------------------------------------------------------------
        public int DeleteUserLanguages(int userId)
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

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spDeleteUserLanguages",
                con,
                paramDic);

            try
            {
                return cmd.ExecuteNonQuery();
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


        //==================================================================================================
        // USER WANTED COUNTRIES
        //==================================================================================================


        //--------------------------------------------------------------------------------------------------
        // Read Wanted Countries
        //--------------------------------------------------------------------------------------------------
        public List<Country> ReadWantedCountries(int userId)
        {
            SqlConnection con;
            SqlCommand cmd;

            List<Country> countries =
                new List<Country>();

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
                "spReadUserWantedCountries",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Country country = new Country();

                    country.CountryId =
                        Convert.ToInt32(
                            dataReader["CountryId"]);

                    country.Cca3 =
                        dataReader["CCA3"].ToString();

                    country.Name =
                        dataReader["Name"].ToString();

                    country.Capital =
                        dataReader["Capital"].ToString();

                    country.Region =
                        new Region(
                            Convert.ToInt32(
                                dataReader["RegionId"]),
                            dataReader["RegionName"].ToString()
                        );

                    country.SubRegion =
                        dataReader["SubRegion"].ToString();

                    country.Population =
                        Convert.ToInt64(
                            dataReader["Population"]);

                    country.Area =
                        Convert.ToDouble(
                            dataReader["Area"]);

                    country.FlagUrl =
                        dataReader["FlagUrl"].ToString();

                    country.Borders =
                        new List<string>(
                            dataReader["Borders"].ToString()
                                .Split(
                                    ',',
                                    StringSplitOptions.RemoveEmptyEntries)
                                .ToList());

                    DBCountryServices countryDB =
                        new DBCountryServices();

                    country.Languages =
                        countryDB.ReadLanguagesByCountryId(
                            country.CountryId);

                    country.Currencies =
                        countryDB.ReadCurrenciesByCountryId(
                            country.CountryId);

                    countries.Add(country);
                }

                return countries;
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
        // Add Wanted Country
        //--------------------------------------------------------------------------------------------------
        public int AddWantedCountry(
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
                "spInsertUserWantedCountry",
                con,
                paramDic);

            try
            {
                return cmd.ExecuteNonQuery();
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
        // Remove Wanted Country
        //--------------------------------------------------------------------------------------------------
        public int RemoveWantedCountry(
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
                "spDeleteUserWantedCountry",
                con,
                paramDic);

            try
            {
                return cmd.ExecuteNonQuery();
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




        public int InsertUserLogin(int userId)
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

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertUserLogin",
                con,
                paramDic);

            try
            {
                int numEffected = cmd.ExecuteNonQuery();

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


        public AdminStatistics ReadStatistics()
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

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadAdminStatistics",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    AdminStatistics statistics =
                        new AdminStatistics();

                    statistics.DailyLogins =
                        Convert.ToInt32(
                            dataReader["DailyLogins"]);

                    statistics.ImportedCountries =
                        Convert.ToInt32(
                            dataReader["ImportedCountries"]);

                    statistics.SavedCountries =
                        Convert.ToInt32(
                            dataReader["SavedCountries"]);

                    statistics.SharedReviews =
                        Convert.ToInt32(
                            dataReader["SharedReviews"]);

                    return statistics;
                }

                return null;
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