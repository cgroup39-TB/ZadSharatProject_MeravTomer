namespace ServerSideCountriesProject_MeravTomer.BL
{

    public class Currency
    {
        private int currencyId;
        private string currencyCode;
        private string name;
        private string symbol;
        private List<Country> countriesAcceptsCoin;//NEW

        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }  // לפעמים אין symbol


        public Currency(int id, string code, string name, string symbol)
        {
            CurrencyId = id;
            CurrencyCode = code;
            Name = name;
            Symbol = symbol;
        }

        public Currency() { }


    }




}


