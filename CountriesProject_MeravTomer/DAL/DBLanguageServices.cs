using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    /// <summary>
    /// Data access layer for Language reference data (the languages spoken in countries).
    /// </summary>
    public class DBLanguageServices
    {
        /// <summary>
        /// Creates a new instance of the service. Holds no state; a fresh DB connection is opened per call.
        /// </summary>
        public DBLanguageServices()
        {
        }


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
        // Read all Languages
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns every language in the database.
        /// </summary>
        public List<Language> ReadAllLanguages()
        {
            SqlConnection con;
            SqlCommand cmd;
            List<Language> languages = new List<Language>();

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadAllLanguages_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Language language = new Language();

                    language.LanguageId =
                        Convert.ToInt32(dataReader["LanguageId"]);

                    language.LanguageName =
                        dataReader["LanguageName"].ToString();

                    languages.Add(language);
                }

                return languages;
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
        // Read Language by ID
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the language matching <paramref name="languageId"/>, or null if none exists.
        /// </summary>
        public Language ReadLanguageById(int languageId)
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

            paramDic.Add("@LanguageId", languageId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadLanguageById_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    Language language = new Language();

                    language.LanguageId =
                        Convert.ToInt32(dataReader["LanguageId"]);

                    language.LanguageName =
                        dataReader["LanguageName"].ToString();

                    return language;
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
        // Read Language by Name
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the language whose name matches <paramref name="languageName"/>, or null if none exists.
        /// </summary>
        public Language ReadLanguageByName(string languageName)
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

            paramDic.Add("@LanguageName", languageName);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadLanguageByName_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    Language language = new Language();

                    language.LanguageId =
                        Convert.ToInt32(dataReader["LanguageId"]);

                    language.LanguageName =
                        dataReader["LanguageName"].ToString();

                    return language;
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
        // Insert Language
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts a new language using <paramref name="language"/>'s name.
        /// </summary>
        /// <returns>The database-generated LanguageId of the newly inserted row.</returns>
        public int InsertLanguage(Language language)
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

            paramDic.Add(
                "@LanguageName",
                language.LanguageName);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertLanguage_3MD_TB",
                con,
                paramDic);

            try
            {
                object result =
                    cmd.ExecuteScalar();

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
    }
}
