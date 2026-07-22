using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.Models;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBCountryServices
    {
        public SqlConnection Connect(string connectionStringName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();
            string connectionString = configuration.GetConnectionString(connectionStringName);
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        private SqlCommand CreateStoredProcedureCommand(string spName, SqlConnection con, Dictionary<string, object> parameters)
        {
            SqlCommand cmd = new SqlCommand(spName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10;

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return cmd;
        }

        private static List<string> ParseBorders(object bordersValue)
        {
            if (bordersValue == null || bordersValue is DBNull)
                return new List<string>();

            return bordersValue.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        private static Country MapCountry(SqlDataReader reader)
        {
            return new Country
            {
                CountryId = Convert.ToInt32(reader["CountryId"]),
                CCA3 = reader["CCA3"].ToString(),
                Name = reader["Name"].ToString(),
                Capital = reader["Capital"] as string,
                RegionId = reader["RegionId"] is DBNull ? 0 : Convert.ToInt32(reader["RegionId"]),
                RegionName = reader["RegionName"] as string,
                SubRegion = reader["SubRegion"] as string,
                Population = reader["Population"] is DBNull ? 0 : Convert.ToInt64(reader["Population"]),
                Area = reader["Area"] is DBNull ? 0 : Convert.ToDouble(reader["Area"]),
                FlagUrl = reader["FlagUrl"] as string,
                Borders = ParseBorders(reader["Borders"])
            };
        }

        private static Currency MapCurrency(SqlDataReader reader)
        {
            return new Currency(
                Convert.ToInt32(reader["CurrencyId"]),
                reader["CurrencyCode"].ToString(),
                reader["Name"].ToString(),
                reader["Symbol"] as string);
        }

        private List<Country> GetCountriesFromReader(SqlCommand cmd)
        {
            List<Country> countries = new List<Country>();
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                countries.Add(MapCountry(reader));
            }

            return countries;
        }

        // -- Countries --

        public int InsertCountry(Country country)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@CCA3", country.CCA3 },
                { "@Name", country.Name },
                { "@Capital", (object)country.Capital ?? DBNull.Value },
                { "@RegionId", country.RegionId },
                { "@SubRegion", (object)country.SubRegion ?? DBNull.Value },
                { "@Population", country.Population },
                { "@Area", country.Area },
                { "@FlagUrl", (object)country.FlagUrl ?? DBNull.Value },
                { "@Borders", string.Join(",", country.Borders) }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_Insert", con, parameters);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Country> GetAllCountries()
        {
            using SqlConnection con = Connect("myProjDB");
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_GetAll", con, null);
            return GetCountriesFromReader(cmd);
        }

        public Country GetCountryById(int countryId)
        {
            Country country;

            using (SqlConnection con = Connect("myProjDB"))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object> { { "@CountryId", countryId } };
                using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_GetById", con, parameters);
                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                country = MapCountry(reader);
            }

            country.Languages = GetLanguagesByCountryId(countryId);
            country.Currencies = GetCurrenciesByCountryId(countryId);
            return country;
        }

        public Country GetCountryByName(string name)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@Name", name } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_GetByName", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            return reader.Read() ? MapCountry(reader) : null;
        }

        public List<Country> SearchCountriesByName(string namePart)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@NamePart", namePart } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_SearchByName", con, parameters);
            return GetCountriesFromReader(cmd);
        }

        public List<Country> GetCountriesByRegion(string regionName)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@RegionName", regionName } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_GetByRegion", con, parameters);
            return GetCountriesFromReader(cmd);
        }

        public List<Country> GetCountriesByLanguage(string languageName)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@LanguageName", languageName } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_GetByLanguage", con, parameters);
            return GetCountriesFromReader(cmd);
        }

        public List<Country> GetCountriesByCurrency(string currencyCode)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@CurrencyCode", currencyCode } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_GetByCurrency", con, parameters);
            return GetCountriesFromReader(cmd);
        }

        public int UpdateCountry(Country country)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@CountryId", country.CountryId },
                { "@CCA3", country.CCA3 },
                { "@Name", country.Name },
                { "@Capital", (object)country.Capital ?? DBNull.Value },
                { "@RegionId", country.RegionId },
                { "@SubRegion", (object)country.SubRegion ?? DBNull.Value },
                { "@Population", country.Population },
                { "@Area", country.Area },
                { "@FlagUrl", (object)country.FlagUrl ?? DBNull.Value },
                { "@Borders", string.Join(",", country.Borders) }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_Update", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int DeleteCountry(int countryId)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@CountryId", countryId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Country_Delete", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        // -- Regions --
        // Only atomic reads/writes here - resolving a region NAME to an ID (and creating it
        // the first time it's seen) takes more than one step, so that coordination belongs
        // in CountryBL, not here.

        public Region GetRegionByName(string regionName)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@RegionName", regionName } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Region_GetByName", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            return reader.Read()
                ? new Region(Convert.ToInt32(reader["RegionId"]), reader["RegionName"].ToString())
                : null;
        }

        public int InsertRegion(string regionName)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@RegionName", regionName } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Region_Insert", con, parameters);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Region> GetAllRegions()
        {
            List<Region> regions = new List<Region>();

            using SqlConnection con = Connect("myProjDB");
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Region_GetAll", con, null);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                regions.Add(new Region(Convert.ToInt32(reader["RegionId"]), reader["RegionName"].ToString()));
            }

            return regions;
        }

        // -- Languages --

        public List<Language> GetAllLanguages()
        {
            List<Language> languages = new List<Language>();

            using SqlConnection con = Connect("myProjDB");
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Language_GetAll", con, null);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                languages.Add(new Language(Convert.ToInt32(reader["LanguageId"]), reader["LanguageName"].ToString()));
            }

            return languages;
        }

        public Language GetLanguageByName(string languageName)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@LanguageName", languageName } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Language_GetByName", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            return reader.Read()
                ? new Language(Convert.ToInt32(reader["LanguageId"]), reader["LanguageName"].ToString())
                : null;
        }

        public int InsertLanguage(string languageName)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@LanguageName", languageName } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Language_Insert", con, parameters);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // -- Currencies --

        public List<Currency> GetAllCurrencies()
        {
            List<Currency> currencies = new List<Currency>();

            using SqlConnection con = Connect("myProjDB");
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Currency_GetAll", con, null);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                currencies.Add(MapCurrency(reader));
            }

            return currencies;
        }

        public Currency GetCurrencyByCode(string currencyCode)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@CurrencyCode", currencyCode } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Currency_GetByCode", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            return reader.Read() ? MapCurrency(reader) : null;
        }

        public int InsertCurrency(Currency currency)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@CurrencyCode", currency.CurrencyCode },
                { "@Name", currency.Name },
                { "@Symbol", currency.Symbol }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_Currency_Insert", con, parameters);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // -- Country <-> Language / Currency links --

        public List<Language> GetLanguagesByCountryId(int countryId)
        {
            List<Language> languages = new List<Language>();

            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@CountryId", countryId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_CountryLanguages_GetByCountryId", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                languages.Add(new Language(Convert.ToInt32(reader["LanguageId"]), reader["LanguageName"].ToString()));
            }

            return languages;
        }

        public int InsertCountryLanguage(int countryId, int languageId)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@CountryId", countryId },
                { "@LanguageId", languageId }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_CountryLanguages_Insert", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int DeleteCountryLanguages(int countryId)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@CountryId", countryId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_CountryLanguages_DeleteByCountryId", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public List<Currency> GetCurrenciesByCountryId(int countryId)
        {
            List<Currency> currencies = new List<Currency>();

            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@CountryId", countryId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_CountryCurrencies_GetByCountryId", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                currencies.Add(MapCurrency(reader));
            }

            return currencies;
        }

        public int InsertCountryCurrency(int countryId, int currencyId)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@CountryId", countryId },
                { "@CurrencyId", currencyId }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_CountryCurrencies_Insert", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int DeleteCountryCurrencies(int countryId)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@CountryId", countryId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_CountryCurrencies_DeleteByCountryId", con, parameters);
            return cmd.ExecuteNonQuery();
        }
    }
}
