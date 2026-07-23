using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using ServerSideCountriesProject_MeravTomer.BL;
using ServerSideCountriesProject_MeravTomer.DAL;

using System.Diagnostics.Metrics;


namespace ServerSideCountriesProject_MeravTomer.DAL// ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBCountryServices
    {

        public DBCountryServices()
        {
        }

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


        //--------------------------------------------------------------------------------------------------
        // Returning a list of all countries
        //--------------------------------------------------------------------------------------------------
        public List<Country> ReadAllCountries()
        {
            SqlConnection con;
            SqlCommand cmd;
            List<Country> countries = new List<Country>();

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadAllCountries_MD_TB2",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Country c = new Country();

                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.Cca3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Capital = dataReader["Capital"].ToString();

                    c.Region = new Region(
                        Convert.ToInt32(dataReader["RegionId"]),
                        dataReader["RegionName"].ToString()
                    );

                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();

                    c.Languages =
                        new List<Language>(
                            ReadLanguagesByCountryId(c.CountryId));

                    c.Currencies =
                        new List<Currency>(
                            ReadCurrenciesByCountryId(c.CountryId));

                    c.Borders = new List<string>(
                        dataReader["Borders"].ToString()
                            .Split(
                                ',',
                                StringSplitOptions.RemoveEmptyEntries)
                            .ToList());

                    countries.Add(c);
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
        // Read country by ID
        //--------------------------------------------------------------------------------------------------
        public Country ReadCountryById(int id)
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

            paramDic.Add("@Id", id);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadCountryById_MD_TB2",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    Country c = new Country();

                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.Cca3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Capital = dataReader["Capital"].ToString();

                    c.Region = new Region(
                        Convert.ToInt32(dataReader["RegionId"]),
                        dataReader["RegionName"].ToString()
                    );

                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();

                    c.Languages =
                        new List<Language>(
                            ReadLanguagesByCountryId(c.CountryId));

                    c.Currencies =
                        new List<Currency>(
                            ReadCurrenciesByCountryId(c.CountryId));

                    c.Borders = new List<string>(
                        dataReader["Borders"].ToString()
                            .Split(
                                ',',
                                StringSplitOptions.RemoveEmptyEntries)
                            .ToList());

                    return c;
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
        // Read country by name
        //--------------------------------------------------------------------------------------------------
        public Country ReadCountryByName(string countryName)
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

            paramDic.Add("@Name", countryName);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadCountryByName",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.Read())
                {
                    Country c = new Country();

                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.Cca3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Capital = dataReader["Capital"].ToString();

                    c.Region = new Region(
                        Convert.ToInt32(dataReader["RegionId"]),
                        dataReader["RegionName"].ToString()
                    );

                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();

                    c.Languages =
                        new List<Language>(
                            ReadLanguagesByCountryId(c.CountryId));

                    c.Currencies =
                        new List<Currency>(
                            ReadCurrenciesByCountryId(c.CountryId));

                    c.Borders = new List<string>(
                        dataReader["Borders"].ToString()
                            .Split(
                                ',',
                                StringSplitOptions.RemoveEmptyEntries)
                            .ToList());

                    dataReader.Close();

                    return c;
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
        // Read all countries of a specific region
        //--------------------------------------------------------------------------------------------------
        public List<Country> ReadCountriesByRegion(Region region)
        {
            SqlConnection con;
            SqlCommand cmd;
            List<Country> countries = new List<Country>();

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

            paramDic.Add("@RegionId", region.RegionId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadCountriesByRegion",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Country c = new Country();

                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.Cca3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Capital = dataReader["Capital"].ToString();

                    c.Region = new Region(
                        Convert.ToInt32(dataReader["RegionId"]),
                        dataReader["RegionName"].ToString()
                    );

                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();

                    c.Languages =
                        new List<Language>(
                            ReadLanguagesByCountryId(c.CountryId));

                    c.Currencies =
                        new List<Currency>(
                            ReadCurrenciesByCountryId(c.CountryId));

                    c.Borders = new List<string>(
                        dataReader["Borders"].ToString()
                            .Split(
                                ',',
                                StringSplitOptions.RemoveEmptyEntries)
                            .ToList());

                    countries.Add(c);
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
        // Insert Country
        //--------------------------------------------------------------------------------------------------
        public int InsertCountry(Country country)
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

            paramDic.Add("@CCA3", country.Cca3);
            paramDic.Add("@Name", country.Name);
            paramDic.Add("@Capital", country.Capital);
            paramDic.Add("@RegionId", country.Region.RegionId);
            paramDic.Add("@SubRegion", country.SubRegion);
            paramDic.Add("@Population", country.Population);
            paramDic.Add("@Area", country.Area);
            paramDic.Add("@FlagUrl", country.FlagUrl);
            paramDic.Add("@Borders", string.Join(",", country.Borders));

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertCountry_MD_TB2",
                con,
                paramDic);

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


        //--------------------------------------------------------------------------------------------------
        // Update Country
        // Languages and currencies are updated separately
        //--------------------------------------------------------------------------------------------------
        public int UpdateCountry(int countryId, Country country)
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

            paramDic.Add("@Id", countryId);
            paramDic.Add("@CCA3", country.Cca3);
            paramDic.Add("@Name", country.Name);
            paramDic.Add("@Capital", country.Capital);
            paramDic.Add("@RegionId", country.Region.RegionId);
            paramDic.Add("@SubRegion", country.SubRegion);
            paramDic.Add("@Population", country.Population);
            paramDic.Add("@Area", country.Area);
            paramDic.Add("@FlagUrl", country.FlagUrl);
            paramDic.Add("@Borders", string.Join(",", country.Borders));

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spUpdateCountry",
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


        //--------------------------------------------------------------------------------------------------
        // Delete Country
        //--------------------------------------------------------------------------------------------------
        public int DeleteCountry(int countryId)
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

            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spDeleteCountry",
                con,
                paramDic);

            try
            {
                DeleteLanguageByCountryIdWhenDeletingCountry(countryId);
                DeleteCurrencyByCountryIdWhenDeletingCountry(countryId);

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


        //--------------------------------------------------------------------------------------------------
        // Read all Languages
        //--------------------------------------------------------------------------------------------------
        public List<Language> ReadAllLanguages()
        {
            SqlConnection con;
            SqlCommand cmd;
            List<Language> lenguages = new List<Language>();

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
                    Language l = new Language();

                    l.LanguageId =
                        Convert.ToInt32(dataReader["LanguageId"]);

                    l.LanguageName =
                        dataReader["LanguageName"].ToString();

                    lenguages.Add(l);
                }

                return lenguages;
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

            paramDic.Add("@LanguageName", language.LanguageName);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertLanguage_MD_TB2",
                con,
                paramDic);

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


        //--------------------------------------------------------------------------------------------------
        // Read all Currencies
        //--------------------------------------------------------------------------------------------------
        public List<Currency> ReadAllCurrencies()
        {
            SqlConnection con;
            SqlCommand cmd;
            List<Currency> currencies = new List<Currency>();

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadAllCurrencies_MD_TB2",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Currency c = new Currency();

                    c.CurrencyId =
                        Convert.ToInt32(dataReader["CurrencyId"]);

                    c.CurrencyCode =
                        dataReader["CurrencyCode"].ToString();

                    c.Name =
                        dataReader["Name"].ToString();

                    c.Symbol =
                        dataReader["Symbol"].ToString();

                    currencies.Add(c);
                }

                return currencies;
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
        // Insert Currency
        //--------------------------------------------------------------------------------------------------
        public int InsertCurrency(Currency currency)
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

            paramDic.Add("@Code", currency.CurrencyCode);
            paramDic.Add("@Name", currency.Name);
            paramDic.Add("@Symbol", currency.Symbol);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertCurrency_MD_TB2",
                con,
                paramDic);

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


        //--------------------------------------------------------------------------------------------------
        // Read languages of a specific Country
        //--------------------------------------------------------------------------------------------------
        public List<Language> ReadLanguagesByCountryId(int countryId)
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

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "sp_CountryLanguages_GetByCountryId",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    int code =
                        Convert.ToInt32(dataReader["LanguageId"]);

                    string name =
                        dataReader["LanguageName"].ToString();

                    languages.Add(
                        new Language(code, name));
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
        // Insert Country-Language relations
        //--------------------------------------------------------------------------------------------------
        public void InsertCountryLanguages(
            int countryId,
            List<Language> languages)
        {
            if (languages == null || languages.Count == 0)
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
                foreach (Language language in languages)
                {
                    Dictionary<string, object> paramDic =
                        new Dictionary<string, object>();

                    paramDic.Add("@CountryId", countryId);
                    paramDic.Add("@LanguageId", language.LanguageId);

                    SqlCommand cmd =
                        CreateCommandWithStoredProcedureGeneral(
                            "sp_CountryLanguages_Insert",
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
        // Delete Country-Language relations
        //--------------------------------------------------------------------------------------------------
        public int DeleteLanguageByCountryIdWhenDeletingCountry(
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

            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spDeleteLanguageByCountryId_MD_TB2",
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


        //--------------------------------------------------------------------------------------------------
        // Read currencies of a specific Country
        //--------------------------------------------------------------------------------------------------
        public List<Currency> ReadCurrenciesByCountryId(int countryId)
        {
            SqlConnection con;
            SqlCommand cmd;
            List<Currency> currencies = new List<Currency>();

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
                "sp_CountryCurrencies_GetByCountryId",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    int id =
                        Convert.ToInt32(dataReader["CurrencyId"]);

                    string code =
                        dataReader["CurrencyCode"].ToString();

                    string name =
                        dataReader["CurrencyName"].ToString();

                    string symbol =
                        dataReader["CurrencySymbol"].ToString();

                    currencies.Add(
                        new Currency(
                            id,
                            code,
                            name,
                            symbol));
                }

                return currencies;
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
        // Insert Country-Currency relations
        //--------------------------------------------------------------------------------------------------
        public void InsertCountryCurrencies(
            int countryId,
            List<Currency> currencies)
        {
            if (currencies == null || currencies.Count == 0)
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
                foreach (Currency currency in currencies)
                {
                    Dictionary<string, object> paramDic =
                        new Dictionary<string, object>();

                    paramDic.Add("@CountryId", countryId);
                    paramDic.Add("@CurrencyId", currency.CurrencyId);

                    SqlCommand cmd =
                        CreateCommandWithStoredProcedureGeneral(
                            "sp_CountryCurrencies_Insert",
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
        // Delete Country-Currency relations
        //--------------------------------------------------------------------------------------------------
        public int DeleteCurrencyByCountryIdWhenDeletingCountry(
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

            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spDeleteCurrencyByCountryId_MD_TB2",
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


        public List<Country> ReadSortedCountries(
    string sortBy,
    bool ascending)
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

            paramDic.Add("@SortBy", sortBy);
            paramDic.Add("@Ascending", ascending);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadSortedCountries",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(
                        CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Country c = new Country();

                    c.CountryId =
                        Convert.ToInt32(
                            dataReader["dbCountryId"]);

                    c.Cca3 =
                        dataReader["CCA3"].ToString();

                    c.Name =
                        dataReader["Name"].ToString();

                    c.Capital =
                        dataReader["Capital"].ToString();

                    c.Region = new Region(
                        Convert.ToInt32(
                            dataReader["RegionId"]),
                        dataReader["RegionName"].ToString()
                    );

                    c.SubRegion =
                        dataReader["SubRegion"].ToString();

                    c.Population =
                        Convert.ToInt64(
                            dataReader["Population"]);

                    c.Area =
                        Convert.ToDouble(
                            dataReader["Area"]);

                    c.FlagUrl =
                        dataReader["FlagUrl"].ToString();

                    c.Languages =
                        new List<Language>(
                            ReadLanguagesByCountryId(
                                c.CountryId));

                    c.Currencies =
                        new List<Currency>(
                            ReadCurrenciesByCountryId(
                                c.CountryId));

                    c.Borders =
                        new List<string>(
                            dataReader["Borders"]
                                .ToString()
                                .Split(
                                    ',',
                                    StringSplitOptions
                                        .RemoveEmptyEntries)
                                .ToList());

                    countries.Add(c);
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
    }
}

