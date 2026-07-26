using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{

    /// <summary>Business-logic representation of a currency (code, name, symbol).</summary>
    public class Currency
    {
        private int currencyId;
        private string currencyCode;
        private string name;
        private string symbol;

        /// <summary>Creates an empty currency instance (used by model binding/deserialization).</summary>
        public Currency()
        {
        }

        /// <summary>Creates a fully-populated currency.</summary>
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

        /// <summary>Returns every currency in the database.</summary>
        public List<Currency> ReadAllCurrencies()
        {
            DBCountryServices db = new DBCountryServices();

            return db.ReadAllCurrencies();
        }


        /// <summary>Returns the currency matching <paramref name="currencyId"/>, or null if none exists.</summary>
        public Currency ReadCurrencyById(int currencyId)
        {
            DBCurrencyServices db = new DBCurrencyServices();

            return db.ReadCurrencyById(currencyId);
        }


        /// <summary>Returns the currency matching <paramref name="currencyCode"/> (e.g. "USD"), or null if none exists.</summary>
        public Currency ReadCurrencyByCode(string currencyCode)
        {
            DBCurrencyServices db = new DBCurrencyServices();

            return db.ReadCurrencyByCode(currencyCode);
        }
    }




}


