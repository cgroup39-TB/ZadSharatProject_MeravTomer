using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    /// <summary>
    /// ADO.NET data-access layer for User data (authentication, profile, admin permissions,
    /// preferences, wanted-countries wishlist, and statistics), executed entirely via stored procedures.
    /// </summary>
    public class DBUserServices
    {
        /// <summary>
        /// Creates a new instance of the User data-access layer. Holds no state; each call opens and closes its own SQL connection via <see cref="connect"/>.
        /// </summary>
        public DBUserServices()
        {
        }


        //--------------------------------------------------------------------------------------------------
        // Create DB connection
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Opens and returns a new <see cref="SqlConnection"/> using the named connection string looked up from appsettings.json.
        /// </summary>
        /// <param name="conString">The connection-string key to look up in appsettings.json (e.g. "myProjDB").</param>
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


        //==================================================================================================
        // USERS
        //==================================================================================================


        //--------------------------------------------------------------------------------------------------
        // Insert User
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts a new user row via spInsertUser_3MD_TB (password is stored as plaintext, with no hashing) and returns the number of rows affected.
        /// </summary>
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
                "spInsertUser_3MD_TB",
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
        /// <summary>
        /// Returns every user in the system via spReadAllUsers_3MD_TB, including each user's plaintext password field.
        /// </summary>
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
                "spReadAllUsers_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Returns the user matching <paramref name="userId"/> via spReadUserById_3MD_TB, or null if none exists.
        /// </summary>
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
                "spReadUserById_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Returns the user matching <paramref name="email"/> via spReadUserByEmail_3MD_TB, or null if none exists; used by the BL layer for login and registration-uniqueness checks.
        /// </summary>
        /// <returns>The matching <see cref="User"/>, whose password field is plaintext (no hashing is used in this project), or null.</returns>
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
                "spReadUserByEmail_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Returns the user matching <paramref name="name"/> via spReadUserByName_3MD_TB, or null if none exists.
        /// </summary>
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
                "spReadUserByName_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Overwrites the stored fields of the user identified by <paramref name="userToUpdateId"/> with the values in <paramref name="userDetails"/> via spUpdateUser_3MD_TB.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
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
                "spUpdateUser_3MD_TB",
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
        /// <summary>
        /// Returns the regions the given user has marked as preferred, via spReadUserRegions_3MD_TB.
        /// </summary>
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
                "spReadUserRegions_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Replaces the user's preferred regions: unconditionally deletes all existing rows for the user, then re-inserts one row per id in <paramref name="regionIds"/>. If <paramref name="regionIds"/> is null or empty the user is simply left with no preferred regions. The delete and the insert loop are not wrapped in a transaction, so a failure partway through the loop can leave the user with a partial region list.
        /// </summary>
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
                            "spInsertUserRegion_3MD_TB",
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
        /// <summary>
        /// Deletes all preferred-region rows for the given user via spDeleteUserRegions_3MD_TB.
        /// </summary>
        /// <returns>The number of rows deleted.</returns>
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
                "spDeleteUserRegions_3MD_TB",
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
        // UserLanguages contains UserId + Language Object + ProficiencyLevel
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the user's known languages, each paired with an optional proficiency level, via spReadUserLanguages_3MD_TB.
        /// </summary>
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
                "spReadUserLanguages_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    Language language =
                        new Language(
                            Convert.ToInt32(
                                dataReader["LanguageId"]),
                            dataReader["ProficiencyLevel"].ToString()
                        );

                    int ProficiencyLevel = null;

                    if (dataReader["ProficiencyLevel"] != DBNull.Value)
                    {
                        ProficiencyLevel =
                            Convert.ToInt32(
                                dataReader["ProficiencyLevel"]);
                    }

                    UserLanguages userLanguage =
                        new UserLanguages(
                            userId,
                            language,
                            ProficiencyLevel
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
        /// <summary>
        /// Replaces the user's languages: unconditionally deletes all existing rows for the user, then re-inserts one row per entry in <paramref name="languages"/>. If <paramref name="languages"/> is null or empty the user is left with no languages. As with <see cref="UpdatePreferredRegions"/>, the delete and the insert loop are not wrapped in a transaction.
        /// </summary>
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
                        "@ProficiencyLevel",
                        userLanguage.ProficiencyLevel.HasValue
                            ? userLanguage.ProficiencyLevel.Value
                            : DBNull.Value);

                    SqlCommand cmd =
                        CreateCommandWithStoredProcedureGeneral(
                            "spInsertUserLanguage_3MD_TB",
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
        /// <summary>
        /// Deletes all language rows for the given user via spDeleteUserLanguages_3MD_TB.
        /// </summary>
        /// <returns>The number of rows deleted.</returns>
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
                "spDeleteUserLanguages_3MD_TB",
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
        /// <summary>
        /// Returns the user's wanted-countries wishlist via spReadUserWantedCountries_3MD_TB, fully populating each <see cref="Country"/> — including its languages and currencies via separate <see cref="DBCountryServices"/> calls issued per country.
        /// </summary>
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
                "spReadUserWantedCountries_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Adds a country to the user's wishlist via spInsertUserWantedCountry_3MD_TB. UserWantedCountries has a composite primary key on (UserId, CountryId), so inserting a duplicate pair throws a SqlException (error 2627/2601), which the calling controller catches and translates into an HTTP 409 Conflict.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
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
                "spInsertUserWantedCountry_3MD_TB",
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
        /// <summary>
        /// Removes a country from the user's wishlist via spDeleteUserWantedCountry_3MD_TB.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
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
                "spDeleteUserWantedCountry_3MD_TB",
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




        /// <summary>
        /// Records a login event for the given user via spInsertUserLogin_3MD_TB.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
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
                "spInsertUserLogin_3MD_TB",
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


        /// <summary>
        /// Returns aggregate admin-dashboard statistics (daily logins, imported countries, saved countries, shared reviews) via spReadAdminStatistics_3MD_TB, or null if the stored procedure returns no row.
        /// </summary>
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
                "spReadAdminStatistics_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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