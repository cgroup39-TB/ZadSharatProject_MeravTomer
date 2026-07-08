namespace CountriesProject_MeravTomer.BL
{
    public class Country
    {
        private string cca3;
        private string name;
        private string officialName;
        private string capital;
        private string region;
        private string subRegion;
        private int population;
        private double area;
        private double latitude;
        private double longitude;
        private string flagUrl;
        private Dictionary<String,String> languages;
        private Dictionary<String, Currency> currencies;
        private List<string> borders;


        public string Cca3 { get; set; }           
        public string Name { get; set; }
        public string OfficialName { get; set; }
        public List<string> Capital { get; set; }     
        public string Region { get; set; }
        public string SubRegion { get; set; }        
        public long Population { get; set; }           
        public double Area { get; set; }              
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string FlagUrl { get; set; }
        public Dictionary<string, string> Languages { get; set; }     
        public Dictionary<string, Currency> Currencies { get; set; }   
        public List<string> Borders { get; set; }   
        

        public Country()
        {
    
        }

        public Country(string cca3, string name, string officialName, string capital, string region, string subregion, int population, double area,
                      double latitude, double longitude, string flagUrl, Dictionary<String, String> languages, Dictionary<String, Currency> currencies, List<string> borders)
        {
            Cca3 = cca3;
            Name = name;
            OfficialName = officialName;
            Capital = new List<string> { capital };
            Region = region;
            SubRegion = subregion;
            Population = population;
            Area = area;
            Latitude = latitude;
            Longitude = longitude;
            FlagUrl = flagUrl;
            Languages = new Dictionary<string, string>();
            Currencies = new Dictionary<string, Currency>();
            Borders = new List<string> ;
        }



       

                

        }

        
    }
    
    
    
    
    
    
    public class Currency
    {
        public string Name { get; set; }
        public string? Symbol { get; set; }  // לפעמים אין symbol
    }
}
