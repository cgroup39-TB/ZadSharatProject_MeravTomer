namespace ServerSideCountriesProject_MeravTomer.Models
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

        public Currency()
        {
        }

        public Currency(int currencyId, string currencyCode, string name, string symbol)
        {
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            Name = name;
            Symbol = symbol;
        }
    }
}
