using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using ServerSideCountriesProject_MeravTomer.BL;
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
        // Returning a list of all countries in the CountriesTable
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

            cmd = CreateCommandWithStoredProcedureGeneral("spReadAllCountries_MD_TB2", con, null);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
           

                    Country c = new Country();
                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.CCA3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Capital = dataReader["Capital"].ToString();
                    c.Region = dataReader["Region"].ToString();
                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();
                    c.Borders = new List<string>(dataReader["Borders"].ToString()
                                                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
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
        // This method reading a specific country by its countryId from the dataBase
        //--------------------------------------------------------------------------------------------------
         public Country ReadCountryById(int id) { 

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
                paramDic.Add("@Id", id);

                cmd = CreateCommandWithStoredProcedureGeneral("spReadCountryById_MD_TB2", con, paramDic);

                try
                {
                    SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    if (dataReader.Read())
                    {
                        Country c = new Country();
                        c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                        c.CCA3 = dataReader["CCA3"].ToString();
                        c.Name = dataReader["Name"].ToString();
                        c.Capital = dataReader["Capital"].ToString();
                        c.Region = dataReader["Region"].ToString();
                        c.SubRegion = dataReader["SubRegion"].ToString();
                        c.Population = Convert.ToInt64(dataReader["Population"]);
                        c.Area = Convert.ToDouble(dataReader["Area"]);
                        c.FlagUrl = dataReader["FlagUrl"].ToString();
                        c.Borders = new List<string>(dataReader["Borders"].ToString()
                                                       .Split(',', StringSplitOptions.RemoveEmptyEntries)
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
        // This method reading a specific country by its name from the dataBase
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@Name", countryName);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadCountryByName_MD_TB2",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.Read())
                {
                    Country c = new Country();
                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.CCA3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Capital = dataReader["Capital"].ToString();
                    c.Region = dataReader["Region"].ToString();
                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();
                    c.Borders = new List<string>(dataReader["Borders"].ToString()
                                                      .Split(',', StringSplitOptions.RemoveEmptyEntries)
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
                con.Close();
            }
        }


        //--------------------------------------------------------------------------------------------------
        // This method Reads all countries of a specific region
        //--------------------------------------------------------------------------------------------------
        public List<Country> ReadCountriesByRegion(string countryRegion)
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@Region", countryRegion);

            cmd = CreateCommandWithStoredProcedureGeneral("spReadCountriesByRegion_MD_TB2", con, paramDic);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Country c = new Country();

                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.CCA3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Capital = dataReader["Capital"].ToString();
                    c.Region = dataReader["Region"].ToString();
                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();
                    c.Borders = new List<string>(dataReader["Borders"].ToString()
                                                      .Split(',', StringSplitOptions.RemoveEmptyEntries)
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@CCA3", country.CCA3);
            paramDic.Add("@Name", country.Name);
            paramDic.Add("@OfficialName", country.OfficialName);
            paramDic.Add("@Capital", country.Capital);
            paramDic.Add("@Region", country.Region);
            paramDic.Add("@SubRegion", country.SubRegion);
            paramDic.Add("@Population", country.Population);
            paramDic.Add("@Area", country.Area);
            //paramDic.Add("@Latitude", country.Latitude);
            //paramDic.Add("@Longitude", country.Longitude);
            paramDic.Add("@FlagUrl", country.FlagUrl);
            paramDic.Add("@Borders", string.Join(",", country.Borders));

            cmd = CreateCommandWithStoredProcedureGeneral("spInsertCountry_MD_TB2", con, paramDic);

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



        ////--------------------------------------------------------------------------------------------------
        //// Updates a country in the countryTable (updating len curr and  will do seperately)
        ////--------------------------------------------------------------------------------------------------
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();

            paramDic.Add("@Id", countryId);
            paramDic.Add("@CCA3", country.CCA3);
            paramDic.Add("@Name", country.Name);
            paramDic.Add("@OfficialName", country.OfficialName);
            paramDic.Add("@Capital", country.Capital);
            paramDic.Add("@Region", country.Region);
            paramDic.Add("SubRegion", country.SubRegion);
            paramDic.Add("@Population", country.Population);
            paramDic.Add("@Area", country.Area);
            paramDic.Add("@FlagUrl", country.FlagUrl);
            paramDic.Add("@Borders", string.Join(",", country.Borders));



            cmd = CreateCommandWithStoredProcedureGeneral("spUpdateCountry_MD_TB2", con, paramDic);

            try
            {
                int numEffected = cmd.ExecuteNonQuery();
                if (numEffected > 0)
                {
                    //DeleteTagsByGameId(gameId);
                    //InsertManyTagsToGame(gameId, game.Tags);
                }

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
        // This method DELETES a game with a specific Id(Not SteamAppID)
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();

            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral("spDeleteCountry_MD_TB2", con, paramDic);

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

            cmd = CreateCommandWithStoredProcedureGeneral("spReadAllLanguages_MD_TB2", con, null);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Language l = new Language();
                    l.Code = dataReader["Code"].ToString();
                    l.Name = dataReader["Name"].ToString();

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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@Code", language.Code);
            paramDic.Add("@Name", language.Name);
          

            cmd = CreateCommandWithStoredProcedureGeneral("spInsertLanguage_MD_TB2", con, paramDic);

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


        public List<Currency> ReadAllCurrencies() {

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

            cmd = CreateCommandWithStoredProcedureGeneral("spReadAllCurrencies_MD_TB2", con, null);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Currency c = new Currency();
                    c.Code = dataReader["Code"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Symbol = dataReader["Symbol"].ToString();

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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@Code", currency.Code);
            paramDic.Add("@Name", currency.Name);
            paramDic.Add("@Symbol", currency.Symbol);


            cmd = CreateCommandWithStoredProcedureGeneral("spInsertCurrency_MD_TB2", con, paramDic);

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
        // This method reading languages of a specific country by its countryId from the dataBase
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral("sp_CountryLanguages_GetByCountryId", con, paramDic);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    string code = dataReader["LanguageCode"].ToString();
                    string name = dataReader["LanguageName"].ToString();
                    languages.Add(new Language(code, name));
                 
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

       

       
         public List<Country> ReadCountriesByLanguage(string language)
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@Language", language); 

            cmd = CreateCommandWithStoredProcedureGeneral("spReadCountriesByLanguage_MD_TB2", con, paramDic);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Country c = new Country();

                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.CCA3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.Capital = dataReader["Capital"].ToString();
                    c.Region = dataReader["Region"].ToString();
                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();
                    c.Borders = new List<string>(dataReader["Borders"].ToString()
                                                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
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
        // This method inserts a country to the CountryTable(GamesTable_MD_TB2) 
        //--------------------------------------------------------------------------------------------------
      
        public void InsertCountryLanguages(int countryId, List<Language> languages)
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
                    Dictionary<string, object> paramDic = new Dictionary<string, object>();
                    paramDic.Add("@CountryId", countryId);
                    paramDic.Add("@LanguageCode", language.Code);
                    paramDic.Add("@LanguageName", language.Name);

                    SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("sp_CountryLanguages_Insert", con, paramDic);
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


        ////--------------------------------------------------------------------------------------------------
        //// delete a county-language relation by the countryId when deleting a country
        ////--------------------------------------------------------------------------------------------------
        public int DeleteLanguageByCountryIdWhenDeletingCountry(int countryId)
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
        // This method reading currencies of a specific country by its countryId from the dataBase
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@CountryId", countryId);

            cmd = CreateCommandWithStoredProcedureGeneral("sp_CountryCurrencies_GetByCountryId", con, paramDic);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    string code = dataReader["CurrencyCode"].ToString();
                    string name = dataReader["CurrencyName"].ToString();
                    string symbol = dataReader["CurrencySymbol"].ToString();

                    currencies.Add(new Currency(code, name, symbol));
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




        public List<Country> ReadCountriesByCurrency(string currency)
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

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@Currency", currency); 

            cmd = CreateCommandWithStoredProcedureGeneral("spReadCountriesBycurrency_MD_TB2", con, paramDic);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Country c = new Country();

                    c.CountryId = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.CCA3 = dataReader["CCA3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.OfficialName = dataReader["OfficialName"].ToString();
                    c.Capital = dataReader["Capital"].ToString();
                    c.Region = dataReader["Region"].ToString();
                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt64(dataReader["Population"]);
                    c.Area = Convert.ToDouble(dataReader["Area"]);
                    c.FlagUrl = dataReader["FlagUrl"].ToString();
                    c.Borders = new List<string>(dataReader["Borders"].ToString()
                                                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
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



        public void InsertCountryCurrencies(int countryId, List<Currency> currencies)
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
                    Dictionary<string, object> paramDic = new Dictionary<string, object>();
                    paramDic.Add("@CountryId", countryId);
                    paramDic.Add("@CurrencyCode", currency.Code);
                    paramDic.Add("@CurrencyName", currency.Name);
                    paramDic.Add("@CurrencySymbol", currency.Symbol);

                    SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("sp_CountryCurrencies_Insert", con, paramDic);
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


        ////--------------------------------------------------------------------------------------------------
        //// delete a county-currency relation by the countryId when deleting a country
        ////--------------------------------------------------------------------------------------------------
        public int DeleteCurrencyByCountryIdWhenDeletingCountry(int countryId)
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
    }
}

