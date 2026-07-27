using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    /// <summary>
    /// ADO.NET data access for currencies. Talks to SQL Server exclusively through stored
    /// procedures (see DAL/SQL_CurrencySP.sql); the "_3MD_TB" suffix on every procedure name
    /// is just this project's naming convention and must match exactly what's defined in SQL,
    /// which was a source of bugs when the suffix drifted between the C# and SQL scripts.
    /// </summary>
    public class DBCurrencyServices
    {
        public DBCurrencyServices()
        {
        }


        /// <summary>Opens (and returns) a new SqlConnection using the named connection string from appsettings.json. Caller is responsible for closing it.</summary>
        public SqlConnection connect(String conString)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string cStr =
                configuration.GetConnectionString(conString);

            SqlConnection connectionToDb =
                new SqlConnection(cStr);

            connectionToDb.Open();

            return connectionToDb;
        }


        private SqlCommand CreateCommandWithStoredProcedureGeneral(
            String spName,
            SqlConnection con,
            Dictionary<string, object> paramDic)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            cmd.CommandText = spName;
            cmd.CommandTimeout = 10;
            cmd.CommandType = CommandType.StoredProcedure;

            if (paramDic != null)
            {
                foreach (KeyValuePair<string, object> param in paramDic)
                {
                    cmd.Parameters.AddWithValue(
                        param.Key,
                        param.Value ?? DBNull.Value);
                }
            }

            return cmd;
        }


        /// <summary>Returns every currency row via spReadAllCurrencies_3MD_TB.</summary>
        public List<Currency> ReadAllCurrencies()
        {
            SqlConnection con;
            SqlCommand cmd;
            List<Currency> currencies = new List<Currency>();

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadAllCurrencies_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Currency currency = new Currency();

                    currency.CurrencyId =
                        Convert.ToInt32(dataReader["CurrencyId"]);

                    currency.CurrencyCode =
                        dataReader["CurrencyCode"].ToString();

                    currency.Name =
                        dataReader["Name"].ToString();

                    currency.Symbol =
                        dataReader["Symbol"].ToString();

                    currencies.Add(currency);
                }

                return currencies;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        /// <summary>Returns the currency matching <paramref name="currencyId"/> via spReadCurrencyById_3MD_TB, or null if not found.</summary>
        public Currency ReadCurrencyById(int currencyId)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@CurrencyId", currencyId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadCurrencyById_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    Currency currency = new Currency();

                    currency.CurrencyId =
                        Convert.ToInt32(dataReader["CurrencyId"]);

                    currency.CurrencyCode =
                        dataReader["CurrencyCode"].ToString();

                    currency.Name =
                        dataReader["Name"].ToString();

                    currency.Symbol =
                        dataReader["Symbol"].ToString();

                    return currency;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        /// <summary>Returns the currency matching <paramref name="currencyCode"/> via spReadCurrencyByCode_3MD_TB, or null if not found.</summary>
        public Currency ReadCurrencyByCode(string currencyCode)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@CurrencyCode", currencyCode);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadCurrencyByCode_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.Read())
                {
                    Currency currency = new Currency();

                    currency.CurrencyId =
                        Convert.ToInt32(dataReader["CurrencyId"]);

                    currency.CurrencyCode =
                        dataReader["CurrencyCode"].ToString();

                    currency.Name =
                        dataReader["Name"].ToString();

                    currency.Symbol =
                        dataReader["Symbol"].ToString();

                    return currency;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        /// <summary>Inserts a new currency via spInsertCurrency_3MD_TB and returns the generated CurrencyId.</summary>
        public int InsertCurrency(Currency currency)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add(
                "@CurrencyCode",
                currency.CurrencyCode);

            paramDic.Add(
                "@Name",
                currency.Name);

            paramDic.Add(
                "@Symbol",
                currency.Symbol);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertCurrency_3MD_TB",
                con,
                paramDic);

            try
            {
                object result =
                    cmd.ExecuteScalar();

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
    }
}