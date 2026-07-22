using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Country
    {
        private int countryId;
        private string cca3; //COUNTRY CODE ISO 3166-1 ALPHA-3
        private string name;
        private string capital;
        private Region region;
        private string subRegion;
        private long population;
        private double area;
        private string flagUrl;
        private List<Language> languages;
        private List<Currency> currencies;
        private List<string> borders;

        public int CountryId { get => countryId; set => countryId = value; }
        public string Cca3 { get => cca3; set => cca3 = value; }
        public string Name { get => name; set => name = value; }
        public string Capital { get => capital; set => capital = value; }
        public Region Region { get => region; set => region = value; }
        public string SubRegion { get => subRegion; set => subRegion = value; }
        public long Population { get => population; set => population = value; }
        public double Area { get => area; set => area = value; }
        public string FlagUrl { get => flagUrl; set => flagUrl = value; }
        public List<Language> Languages { get => languages; set => languages = value; }
        public List<Currency> Currencies { get => currencies; set => currencies = value; }
        public List<string> Borders { get => borders; set => borders = value; }

        public Country()
        {

        }

        public Country(int id, string cca3, string name, string capital, Region region, string subregion, long population, double area,
                       string flagUrl, List<Language> countryLangArr, List<Currency> countryCurrenciesArr, List<string> borders)
        {
            CountryId = id;
            Cca3 = cca3;
            Name = name;
            Capital = capital;
            Region =new Region(region);
            SubRegion = subregion;
            Population = population;
            Area = area;
            FlagUrl = flagUrl;
            Languages = new List<Language>();
            countryLangArr.ForEach(lang => Languages.Add(new Language(lang.LanguageId, lang.LanguageName)));
            Currencies = new List<Currency>();
            countryCurrenciesArr.ForEach(curr => Currencies.Add(new Currency(curr.CurrencyId,curr.CurrencyCode, curr.Name, curr.Symbol)));
            Borders = new List<string>();
            borders.ForEach(border => Borders.Add(border));

        }


        public Country Insert()
        {             // כאן תוכל להוסיף לוגיקה להוספת המדינה למסד הנתונים או לכל מקום אחר שבו אתה רוצה לשמור את המידע.
            // לדוגמה, אם יש לך מחלקת Repository או Service שמטפלת בהוספה למסד הנתונים, תוכל לקרוא לה כאן.
            // לדוגמה:
            // CountryRepository.Add(this);

            DBCountryServices dBCountryServices = new DBCountryServices();
            //  string 

            return this; // מחזיר את האובייקט הנוכחי לאחר ההוספה}
        }

        public List<Country> ReadAllCountries()
        {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.ReadAllCountries();

        }



        public Country ReadCountryById(int countryId)
        {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.ReadCountryById(countryId);

        }

        public Country ReadCountryByName(string countryName)
        {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.ReadCountryByName(countryName);

        }

        public List<Country> ReadCountriesByRegion(string countryRegion)
        {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.ReadCountriesByRegion(countryRegion);

        }

        //public List<Country> ReadCountryByLanguage(string languageName)
        //{

        //}
        //public List<Country> ReadCountryByCurrency(string currency)
        //{

        //}

        //מידע לסינון ומיון
        //public Country ReadByPopulation()
        //{

        //    DBCountryServices dbs = new DBCountryServices();

        //}

        //public Country ReadByArea(double languageName)  
        //{

        //    DBCountryServices dbs = new DBCountryServices();

        //}

        public int UpdateCountry(int countryId, Country updatedCountry) //Beni said if i change database so better to create new object and use the class method else i can use static methods
        {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.UpdateCountry(countryId, updatedCountry);
        }

        public int DeleteCountry(int countryId) //Beni said if i change database so better to create new object and use the class method else i can use static methods
        {
            DBCountryServices dbs = new DBCountryServices();
            return dbs.DeleteCountry(countryId);
        }





    }


}

