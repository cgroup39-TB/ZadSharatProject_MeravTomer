using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBLanguageServices
    {
        public DBLanguageServices()
        {
        }


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
                        param.Value);
                }
            }

            return cmd;
        }


        //--------------------------------------------------------------------------------------------------
        // Read all Languages
        //--------------------------------------------------------------------------------------------------
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
                "spReadAllLanguages",
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
                "spReadLanguageById",
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
                "spReadLanguageByName",
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
                "spInsertLanguage",
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
