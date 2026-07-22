using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Country
    {
        private int countryId;
        private string cca3; //COUNTRY CODE ISO 3166-1 ALPHA-3
        private string name;
        private string capital;
        private string region;
        private string subRegion;
        private int population;
        private double area;
        private string flagUrl;
        private List<Language> languages;
        private List<Currency> currencies;
        private List<string> borders;


        public int CountryId { get => countryId; set => countryId = value; }
        public string CCA3 { get; set; }
        public string Name { get; set; }
        public string Capital { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public long Population { get; set; }
        public double Area { get; set; }
        public string FlagUrl { get; set; }
        public List<Language> Languages { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<string> Borders { get; set; }


        public Country()
        {

        }

        public Country(int id, string cca3, string name, string capital, string region, string subregion, long population, double area,
                       string flagUrl, List<Language> languages, List<Currency> currencies, List<string> borders)
        {
            CountryId = id;
            CCA3 = cca3;
            Name = name;
            Capital = capital;
            Region = region;
            SubRegion = subregion;
            Population = population;
            Area = area;
            FlagUrl = flagUrl;
            Languages = new List<Language>();
            languages.ForEach(lang => Languages.Add(new Language(lang.Code, lang.Name)));
            Currencies = new List<Currency>();
            currencies.ForEach(curr => Currencies.Add(new Currency(curr.Code, curr.Name, curr.Symbol)));
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

        public List<Country> ReadCountryByLanguage(string languageName)
        {

            DBCountryServices dbs = new DBCountryServices();

        }
        public List<Country> ReadCountryByCurrency(string currency)
        {

            DBCountryServices dbs = new DBCountryServices();

        }
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

