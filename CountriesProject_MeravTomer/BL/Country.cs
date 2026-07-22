using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Country
    {
        private int id;
        private string cca3; //COUNTRY CODE ISO 3166-1 ALPHA-3
        private string name;
        private string officialName;
        private string capital;
        private string region;
        private string subRegion;
        private int population;
        private double area;
        private string flagUrl;
        private List<Language> languages;
        private List<Currency> currencies;
        private List<string> borders;


        public int Id { get => id; set => id = value; }
        public string Cca3 { get; set; }
        public string Name { get; set; }
        public string OfficialName { get; set; }
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

        public Country(int id, string cca3, string name, string officialName, string capital, string region, string subregion, long population, double area,
                       string flagUrl, List<Language> languages, List<Currency> currencies, List<string> borders)
        {
            Id = id;
            Cca3 = cca3;
            Name = name;
            OfficialName = officialName;
            Capital = capital ;
            Region = region;
            SubRegion = subregion;
            Population = population;
            Area = area;
            FlagUrl = flagUrl;
            Languages = new List<Language>();
            Currencies = new List<Currency>();
            Borders = new List<string>() ;
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

        public List<Country> Read() {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.ReadAllCountries();

        }



        public Country ReadById(int countryId)
        {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.ReadCountryById(countryId);

        }

        public Country ReadByName(string countryName)
        {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.ReadCountryByName(countryName);

        }

        public List<Country> ReadByRegion(string countryRegion)
        {

            DBCountryServices dbs = new DBCountryServices();
            return dbs.ReadCountriesByRegion(countryRegion);

        }

        public List<Country> ReadByLanguage(string languageName) {

            DBCountryServices dbs = new DBCountryServices();

        }
        public List<Country> ReadByCurrency(string currency)
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

        public int Delete(int countryId) //Beni said if i change database so better to create new object and use the class method else i can use static methods
        {
            DBCountryServices dbs = new DBCountryServices();
            return dbs.DeleteCountry(countryId);
        }





    }




    public class Language
    {
        private string code;
        private string name;
        public string Code { get; set; }
        public string Name { get; set; }

        public Language(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public Language() { }
    }

   
    public class Currency
    {
        private string code;
        private string name;
        private string symbol;

        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }  // לפעמים אין symbol


        public Currency(string code, string name, string symbol)
        {
            Code = code;
            Name = name;
            Symbol = symbol;
        }

        public Currency() { }

        
    }




}
