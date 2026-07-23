using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBCurrencyServices
    {
        public DBCurrencyServices()
        {
        }


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
                        param.Value);
                }
            }

            return cmd;
        }


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
                "spReadAllCurrencies",
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
                "spReadCurrencyById",
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
                "spReadCurrencyByCode",
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
                "spInsertCurrency",
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