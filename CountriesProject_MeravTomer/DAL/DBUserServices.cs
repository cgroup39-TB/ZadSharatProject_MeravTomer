using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.Models;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBUserServices
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

        private static User MapUser(SqlDataReader reader)
        {
            return new User
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                Name = reader["Name"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["Password"].ToString(),
                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CanShare = Convert.ToBoolean(reader["CanShare"])
            };
        }

        public int InsertUser(User user)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Name", user.Name },
                { "@Email", user.Email },
                { "@Password", user.Password },
                { "@IsAdmin", user.IsAdmin },
                { "@IsActive", user.IsActive },
                { "@CanShare", user.CanShare }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_User_Insert", con, parameters);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public User GetUserById(int userId)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@UserId", userId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_User_GetById", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            return reader.Read() ? MapUser(reader) : null;
        }

        public User GetUserByEmail(string email)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@Email", email } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_User_GetByEmail", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            return reader.Read() ? MapUser(reader) : null;
        }

        public int UpdateUserProfile(int userId, string name, string email)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@Name", name },
                { "@Email", email }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_User_UpdateProfile", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int UpdateUserPassword(int userId, string hashedPassword)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@Password", hashedPassword }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_User_UpdatePassword", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public List<UserLanguage> GetUserLanguages(int userId)
        {
            List<UserLanguage> languages = new List<UserLanguage>();

            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@UserId", userId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_UserLanguages_GetByUserId", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                languages.Add(new UserLanguage(
                    Convert.ToInt32(reader["LanguageId"]),
                    reader["LanguageName"].ToString(),
                    reader["ProficiencyLevel"].ToString()));
            }

            return languages;
        }

        public int DeleteUserLanguages(int userId)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@UserId", userId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_UserLanguages_DeleteByUserId", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int InsertUserLanguage(int userId, int languageId, string proficiencyLevel)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@LanguageId", languageId },
                { "@ProficiencyLevel", proficiencyLevel }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_UserLanguages_Insert", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public List<Region> GetPreferredRegions(int userId)
        {
            List<Region> regions = new List<Region>();

            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@UserId", userId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_UserRegions_GetByUserId", con, parameters);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                regions.Add(new Region(
                    Convert.ToInt32(reader["RegionId"]),
                    reader["RegionName"].ToString()));
            }

            return regions;
        }

        public int DeletePreferredRegions(int userId)
        {
            using SqlConnection con = Connect("myProjDB");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@UserId", userId } };
            using SqlCommand cmd = CreateStoredProcedureCommand("sp_UserRegions_DeleteByUserId", con, parameters);
            return cmd.ExecuteNonQuery();
        }

        public int InsertPreferredRegion(int userId, int regionId)
        {
            using SqlConnection con = Connect("myProjDB");

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@RegionId", regionId }
            };

            using SqlCommand cmd = CreateStoredProcedureCommand("sp_UserRegions_Insert", con, parameters);
            return cmd.ExecuteNonQuery();
        }
    }
}
