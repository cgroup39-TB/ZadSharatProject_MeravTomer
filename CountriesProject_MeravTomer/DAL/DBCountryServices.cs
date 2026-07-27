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
    /// <summary>
    /// ADO.NET data-access layer for Country-related data. All database access goes through SQL Server
    /// stored procedures (no inline SQL, no EF); stored procedure names follow the "*_3MD_TB" naming suffix
    /// convention, which must match exactly between this file and the corresponding SQL scripts.
    /// </summary>
    public class DBCountryServices
    {

        /// <summary>
        /// Creates a new instance of the data-access layer. Holds no state; each method opens and closes
        /// its own database connection.
        /// </summary>
        public DBCountryServices()
        {
        }

        //--------------------------------------------------------------------------------------------------
        // This method creates a connection to the database according to the connectionString name in the web.config
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Opens and returns a new <see cref="SqlConnection"/> using the named connection string read from
        /// appsettings.json.
        /// </summary>
        /// <param name="conString">Name of the connection string entry in appsettings.json (e.g. "myProjDB").</param>
        /// <returns>An already-open connection; the caller is responsible for closing it.</returns>
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
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);

                }


            return cmd;
        }


        //--------------------------------------------------------------------------------------------------
        // Returning a list of all countries
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieves every country via the spReadAllCountries_3MD_TB stored procedure, including each
        /// country's languages, currencies, and border list.
        /// </summary>
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
                "spReadAllCountries_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Returns the country matching <paramref name="id"/> via the spReadCountryById_3MD_TB stored
        /// procedure, including its languages, currencies, and borders, or null if no matching country exists.
        /// </summary>
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
                "spReadCountryById_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Returns the country matching <paramref name="countryName"/> via the spReadCountryByName_3MD_TB
        /// stored procedure, including its languages, currencies, and borders, or null if no matching country exists.
        /// </summary>
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
                "spReadCountryByName_3MD_TB",
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
        /// <summary>
        /// Returns all countries belonging to <paramref name="region"/> via the spReadCountriesByRegion_3MD_TB
        /// stored procedure, including each country's languages, currencies, and borders.
        /// </summary>
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
                "spReadCountriesByRegion_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Inserts a new country row via the spInsertCountry_3MD_TB stored procedure. Borders are stored as a
        /// single comma-separated string rather than a separate table.
        /// </summary>
        /// <returns>The newly generated country ID.</returns>
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
                "spInsertCountry_3MD_TB",
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
        /// <summary>
        /// Updates the country identified by <paramref name="countryId"/> with the values in
        /// <paramref name="country"/> via the spUpdateCountry_3MD_TB stored procedure. Languages and
        /// currencies are not touched here and must be updated through the dedicated methods.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
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
                "spUpdateCountry_3MD_TB",
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
        /// <summary>
        /// Deletes a country and its dependent rows via the spDeleteCountry_3MD_TB stored procedure. First
        /// removes the country's language and currency associations so the delete does not violate
        /// foreign-key constraints, then deletes the country row itself.
        /// </summary>
        /// <returns>The number of rows affected by the country delete.</returns>
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
                "spDeleteCountry_3MD_TB",
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
        /// <summary>
        /// Retrieves every language row via the spReadAllLanguages_3MD_TB stored procedure.
        /// </summary>
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
                "spReadAllLanguages_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Inserts a new language row via the spInsertLanguage_3MD_TB stored procedure.
        /// </summary>
        /// <returns>The newly generated language ID.</returns>
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
                "spInsertLanguage_3MD_TB",
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
        /// <summary>
        /// Retrieves every currency row via the spReadAllCurrencies_3MD_TB stored procedure.
        /// </summary>
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
                "spReadAllCurrencies_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Inserts a new currency row via the spInsertCurrency_3MD_TB stored procedure.
        /// </summary>
        /// <returns>The newly generated currency ID.</returns>
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
                "spInsertCurrency_3MD_TB",
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
        /// <summary>
        /// Returns the languages associated with <paramref name="countryId"/> via the
        /// sp_CountryLanguages_GetByCountryId_3MD_TB stored procedure.
        /// </summary>
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
                "sp_CountryLanguages_GetByCountryId_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Inserts one Country-Language association row per entry in <paramref name="languages"/> via the
        /// sp_CountryLanguages_Insert_3MD_TB stored procedure, reusing a single connection for the whole batch.
        /// Does nothing if <paramref name="languages"/> is null or empty. The join table has a composite
        /// (CountryId, LanguageId) key, so inserting a duplicate pair throws a SqlException that the caller
        /// is expected to translate into an HTTP 409 Conflict.
        /// </summary>
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
                            "sp_CountryLanguages_Insert_3MD_TB",
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
        /// <summary>
        /// Removes all Country-Language associations for <paramref name="countryId"/> via the
        /// spDeleteLanguageByCountryId_3MD_TB stored procedure. Used as a pre-step before deleting a country.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
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
                "spDeleteLanguageByCountryId_3MD_TB",
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
        /// <summary>
        /// Returns the currencies associated with <paramref name="countryId"/> via the
        /// sp_CountryCurrencies_GetByCountryId_3MD_TB stored procedure.
        /// </summary>
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
                "sp_CountryCurrencies_GetByCountryId_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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
        /// <summary>
        /// Inserts one Country-Currency association row per entry in <paramref name="currencies"/> via the
        /// sp_CountryCurrencies_Insert_3MD_TB stored procedure, reusing a single connection for the whole batch.
        /// Does nothing if <paramref name="currencies"/> is null or empty. The join table has a composite
        /// (CountryId, CurrencyId) key, so inserting a duplicate pair throws a SqlException that the caller
        /// is expected to translate into an HTTP 409 Conflict.
        /// </summary>
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
                            "sp_CountryCurrencies_Insert_3MD_TB",
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
        /// <summary>
        /// Removes all Country-Currency associations for <paramref name="countryId"/> via the
        /// spDeleteCurrencyByCountryId_3MD_TB stored procedure. Used as a pre-step before deleting a country.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
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
                "spDeleteCurrencyByCountryId_3MD_TB",
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
        /// Returns all countries sorted via the spReadSortedCountries_3MD_TB stored procedure, including
        /// each country's languages, currencies, and borders.
        /// </summary>
        /// <param name="sortBy">Column name to sort by; passed through directly to the stored procedure.</param>
        /// <param name="ascending">True for ascending order, false for descending.</param>
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
                "spReadSortedCountries_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

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

