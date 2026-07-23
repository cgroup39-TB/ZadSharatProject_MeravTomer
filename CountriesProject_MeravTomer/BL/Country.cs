using ServerSideCountriesProject_MeravTomer.DAL;
using System.Text.Json;


namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Country
    {
       
            private int countryId;
            private string cca3;
            private string name;
            private string capital;
            private Region region;
            private string subRegion;
            private long population;
            private double area;
            private string flagUrl;
            private List<string> borders;
            private List<Language> languages;
            private List<Currency> currencies;


            public Country()
            {
            }


            public Country(
                int countryId,
                string cca3,
                string name,
                string capital,
                Region region,
                string subRegion,
                long population,
                double area,
                string flagUrl,
                List<Language> languages,
                List<Currency> currencies,
                List<string> borders)
            {
                CountryId = countryId;
                Cca3 = cca3;
                Name = name;
                Capital = capital;
                Region = region;
                SubRegion = subRegion;
                Population = population;
                Area = area;
                FlagUrl = flagUrl;
                Languages = languages;
                Currencies = currencies;
                Borders = borders;
            }


            // =========================
            // Properties
            // =========================

            public int CountryId
            {
                get => countryId;
                set => countryId = value;
            }

            public string Cca3
            {
                get => cca3;
                set => cca3 = value;
            }

            public string Name
            {
                get => name;
                set => name = value;
            }

            public string Capital
            {
                get => capital;
                set => capital = value;
            }

            public Region Region
            {
                get => region;
                set => region = value;
            }

            public string SubRegion
            {
                get => subRegion;
                set => subRegion = value;
            }

            public long Population
            {
                get => population;
                set => population = value;
            }

            public double Area
            {
                get => area;
                set => area = value;
            }

            public string FlagUrl
            {
                get => flagUrl;
                set => flagUrl = value;
            }

            public List<string> Borders
            {
                get => borders;
                set => borders = value;
            }

            public List<Language> Languages
            {
                get => languages;
                set => languages = value;
            }

            public List<Currency> Currencies
            {
                get => currencies;
                set => currencies = value;
            }


            // =========================
            // CRUD
            // =========================

            public Country Insert()
            {
                DBCountryServices db = new DBCountryServices();

                // DAL InsertCountry(...)
                return this;
            }


            public int UpdateCountry(
                int countryId,
                Country updatedCountry)
            {
                DBCountryServices db = new DBCountryServices();

                return db.UpdateCountry(countryId, updatedCountry);
            }


            public int DeleteCountry(int countryId)
            {
                DBCountryServices db = new DBCountryServices();

                return db.DeleteCountry(countryId);
            }


            // =========================
            // Read
            // =========================

            public List<Country> ReadAllCountries()
            {
                DBCountryServices db = new DBCountryServices();

                return db.ReadAllCountries();
            }


            public Country ReadCountryById(int countryId)
            {
                DBCountryServices db = new DBCountryServices();

                return db.ReadCountryById(countryId);
            }


            public Country ReadCountryByName(string countryName)
            {
                DBCountryServices db = new DBCountryServices();

                return db.ReadCountryByName(countryName);
            }


            public List<Country> ReadCountriesByRegion(Region region)
            {
                DBCountryServices db = new DBCountryServices();

                return db.ReadCountriesByRegion(region);
            }




        public async Task<int> ImportCountriesFromApi()
        {
            string url =
                "https://countries.dev/countries" +
                "?fields=name,alpha3Code,capital,region,subregion," +
                "population,area,borders,languages,currencies,flags,flag";

            using HttpClient client = new HttpClient();

            string json =
                await client.GetStringAsync(url);

            JsonDocument document =
                JsonDocument.Parse(json);

            DBCountryServices countryDb =
                new DBCountryServices();

            DBRegionServices regionDb =
                new DBRegionServices();

            DBLanguageServices languageDb =
                new DBLanguageServices();

            DBCurrencyServices currencyDb =
                new DBCurrencyServices();

            int insertedCountries = 0;

            foreach (JsonElement item
                     in document.RootElement.EnumerateArray())
            {
                string countryName =
                    item.GetProperty("name").GetString();

                // Prevent duplicate import
                Country existingCountry =
                    countryDb.ReadCountryByName(countryName);

                if (existingCountry != null)
                {
                    continue;
                }


                // =============================================
                // REGION
                // =============================================

                string regionName =
                    item.GetProperty("region").GetString();

                Region region =
                    regionDb.ReadRegionByName(regionName);

                if (region == null)
                {
                    Region newRegion =
                        new Region(0, regionName);

                    int regionId =
                        regionDb.InsertRegion(newRegion);

                    region =
                        new Region(
                            regionId,
                            regionName);
                }


                // =============================================
                // LANGUAGES
                // =============================================

                List<Language> languages =
                    new List<Language>();

                if (item.TryGetProperty(
                    "languages",
                    out JsonElement languagesJson))
                {
                    foreach (JsonElement languageJson
                             in languagesJson.EnumerateArray())
                    {
                        string languageName =
                            languageJson
                                .GetProperty("name")
                                .GetString();

                        Language language =
                            languageDb
                                .ReadLanguageByName(
                                    languageName);

                        if (language == null)
                        {
                            Language newLanguage =
                                new Language(
                                    0,
                                    languageName);

                            int languageId =
                                languageDb
                                    .InsertLanguage(
                                        newLanguage);

                            language =
                                new Language(
                                    languageId,
                                    languageName);
                        }

                        languages.Add(language);
                    }
                }


                // =============================================
                // CURRENCIES
                // =============================================

                List<Currency> currencies =
                    new List<Currency>();

                if (item.TryGetProperty(
                    "currencies",
                    out JsonElement currenciesJson))
                {
                    foreach (JsonElement currencyJson
                             in currenciesJson.EnumerateArray())
                    {
                        string currencyCode =
                            currencyJson
                                .GetProperty("code")
                                .GetString();

                        string currencyName =
                            currencyJson
                                .GetProperty("name")
                                .GetString();

                        string symbol =
                            currencyJson.TryGetProperty(
                                "symbol",
                                out JsonElement symbolJson)
                            ? symbolJson.GetString()
                            : null;

                        Currency currency =
                            currencyDb.ReadCurrencyByCode(
                                currencyCode);

                        if (currency == null)
                        {
                            Currency newCurrency =
                                new Currency(
                                    0,
                                    currencyCode,
                                    currencyName,
                                    symbol);

                            int currencyId =
                                currencyDb.InsertCurrency(
                                    newCurrency);

                            currency =
                                new Currency(
                                    currencyId,
                                    currencyCode,
                                    currencyName,
                                    symbol);
                        }

                        currencies.Add(currency);
                    }
                }


                // =============================================
                // BORDERS
                // =============================================

                List<string> borders =
                    new List<string>();

                if (item.TryGetProperty(
                    "borders",
                    out JsonElement bordersJson))
                {
                    foreach (JsonElement border
                             in bordersJson.EnumerateArray())
                    {
                        borders.Add(
                            border.GetString());
                    }
                }


                // =============================================
                // FLAG
                // =============================================

                string flagUrl = "";

                if (item.TryGetProperty(
                    "flags",
                    out JsonElement flagsJson)
                    &&
                    flagsJson.TryGetProperty(
                        "svg",
                        out JsonElement svgJson))
                {
                    flagUrl =
                        svgJson.GetString();
                }


                // =============================================
                // COUNTRY
                // =============================================

                Country country =
                    new Country(
                        0,
                        item.GetProperty("alpha3Code")
                            .GetString(),

                        countryName,

                        item.TryGetProperty(
                            "capital",
                            out JsonElement capitalJson)
                            ? capitalJson.GetString()
                            : null,

                        region,

                        item.TryGetProperty(
                            "subregion",
                            out JsonElement subRegionJson)
                            ? subRegionJson.GetString()
                            : null,

                        item.GetProperty("population")
                            .GetInt64(),

                        item.TryGetProperty(
                            "area",
                            out JsonElement areaJson)
                            &&
                            areaJson.ValueKind !=
                            JsonValueKind.Null
                            ? areaJson.GetDouble()
                            : 0,

                        flagUrl,

                        languages,
                        currencies,
                        borders
                    );


                // =============================================
                // INSERT COUNTRY
                // =============================================

                int newCountryId =
                    countryDb.InsertCountry(country);

                countryDb.InsertCountryLanguages(
                    newCountryId,
                    languages);

                countryDb.InsertCountryCurrencies(
                    newCountryId,
                    currencies);

                insertedCountries++;
            }

            return insertedCountries;
        }


        public List<Country> ReadSortedCountries( string sortBy, bool ascending = true)//true = ascending, false = descending
        {
            DBCountryServices db =
                new DBCountryServices();

            return db.ReadSortedCountries(sortBy,ascending);
        }

    }



    }



