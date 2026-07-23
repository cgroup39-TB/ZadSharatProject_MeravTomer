using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.Models;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBVisitedCountriesServices
    {
        public SqlConnection Connect(string connectionStringName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();
            string connectionString = configuration.GetConnectionString(connectionStringName);
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        private SqlCommand CreateStoredProcedureCommand(string spName, SqlConnection con, Dictionary<string, object> parameters)
        {
            SqlCommand cmd = new SqlCommand(spName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10;

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return cmd;
        }

        private static List<string> ParseBorders(object bordersValue)
        {
            if (bordersValue == null || bordersValue is DBNull)
                return new List<string>();

            return bordersValue.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        private static UserVisitedCountry MapVisitedCountry(SqlDataReader reader)
        {
            return new UserVisitedCountry
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                CountryId = Convert.ToInt32(reader["CountryId"]),
                Rating = reader["Rating"] is DBNull ? null : Convert.ToInt32(reader["Rating"]),
                ReviewText = reader["ReviewText"] as string,
                IsShared = Convert.ToBoolean(reader["IsShared"]),
                Country = new Country
                {
                    CountryId = Convert.ToInt32(reader["CountryId"]),
                    CCA3 = reader["CCA3"].ToString(),
                    Name = reader["Name"].ToString(),
                    Capital = reader["Capital"] as string,
                    RegionId = reader["RegionId"] is DBNull ? 0 : Convert.ToInt32(reader["RegionId"]),
                    RegionName = reader["RegionName"] as string,
                    SubRegion = reader["SubRegion"] as string,
                    Population = reader["Population"] is DBNull ? 0 : Convert.ToInt64(reader["Population"]),
                    Area = reader["Area"] is DBNull ? 0 : Convert.ToDouble(reader["Area"]),
                    FlagUrl = reader["FlagUrl"] as string,
                    Borders = ParseBorders(reader["Borders"])
                }
            };
        }

        public List<UserVisitedCountry> GetVisitedCountries(int userId)
        {
            List<UserVisitedCountry> visitedCountries = new List<UserVisitedCountry>();

            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@UserId", userId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_VisitedCountries_GetByUserId", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                visitedCountries.Add(MapVisitedCountry(reader));
            }

            return visitedCountries;
        }

        public bool IsCountryVisited(int userId, int countryId)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@CountryId", countryId }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_VisitedCountries_Exists", con, parameters);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        public int InsertVisitedCountry(int userId, int countryId)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@CountryId", countryId }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_VisitedCountries_Insert", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int DeleteVisitedCountry(int userId, int countryId)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@CountryId", countryId }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_VisitedCountries_Delete", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int UpdateRating(int userId, int countryId, int rating)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@CountryId", countryId },
                { "@Rating", rating }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_VisitedCountries_UpdateRating", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int UpdateReview(int userId, int countryId, string reviewText)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@CountryId", countryId },
                { "@ReviewText", (object)reviewText ?? DBNull.Value }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_VisitedCountries_UpdateReview", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int UpdateIsShared(int userId, int countryId, bool isShared)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@CountryId", countryId },
                { "@IsShared", isShared }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_VisitedCountries_UpdateIsShared", con, parameters);
            return cmd.ExecuteNonQuery();
        }
    }
}
