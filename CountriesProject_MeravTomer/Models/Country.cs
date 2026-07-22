namespace ServerSideCountriesProject_MeravTomer.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CCA3 { get; set; }
        public string Name { get; set; }
        public string Capital { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string SubRegion { get; set; }
        public long Population { get; set; }
        public double Area { get; set; }
        public string FlagUrl { get; set; }
        public List<string> Borders { get; set; } = new List<string>();
        public List<Language> Languages { get; set; } = new List<Language>();
        public List<Currency> Currencies { get; set; } = new List<Currency>();

        public Country()
        {
        }
    }
}
