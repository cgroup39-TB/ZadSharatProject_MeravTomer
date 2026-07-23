using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{

    public class Currency
    {
        private int currencyId;
        private string currencyCode;
        private string name;
        private string symbol;

        public Currency()
        {
        }

        public Currency(
            int currencyId,
            string currencyCode,
            string name,
            string symbol)
        {
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            Name = name;
            Symbol = symbol;
        }

        public int CurrencyId
        {
            get => currencyId;
            set => currencyId = value;
        }

        public string CurrencyCode
        {
            get => currencyCode;
            set => currencyCode = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Symbol
        {
            get => symbol;
            set => symbol = value;
        }


        // =========================
        // Read
        // =========================

        public List<Currency> ReadAllCurrencies()
        {
            DBCountryServices db = new DBCountryServices();

            return db.ReadAllCurrencies();
        }


        public Currency ReadCurrencyById(int currencyId)
        {
            DBCountryServices db = new DBCountryServices();

            return db.ReadCurrencyById(currencyId);
        }


        public Currency ReadCurrencyByCode(string currencyCode)
        {
            DBCountryServices db = new DBCountryServices();

            return db.ReadCurrencyByCode(currencyCode);
        }
    }




}


