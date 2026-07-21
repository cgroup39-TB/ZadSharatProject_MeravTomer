using System;
using CountriesProject_MeravTomer.BL;
using System.Data;
using System.Data.SqlClient;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class DBUserServices
    {
        public DBUserServices() { }
    }

    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the web.config
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {

        // read the connection string from the configuration file
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json").Build();
        string cStr = configuration.GetConnectionString(conString);
        SqlConnection connectionToDb = new SqlConnection(cStr);
        connectionToDb.Open();
        return connectionToDb;
    }

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGeneral(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }

    //Insert User into the db
    public int InsertUser(User user)
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

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Name", user.Name);
        paramDic.Add("@Email", user.Email);
        paramDic.Add("@Password", user.Password);
        paramDic.Add("@Active", user.Active);

        cmd = CreateCommandWithStoredProcedureGeneral("spInsertUser_MD_TB2", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
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

    //Reads ALL Users in the db
    public List<User> ReadAllUsers()
    {
        SqlConnection con;
        SqlCommand cmd;
        List<User> users = new List<User>();

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        cmd = CreateCommandWithStoredProcedureGeneral("spReadAllUsers_MD_TB2", con, null);

        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                User u = new User();

                u.Id = Convert.ToInt32(dataReader["dbUserId"]);
                u.Name = dataReader["Name"].ToString();
                u.Email = dataReader["Email"].ToString();
                u.Password = dataReader["Password"].ToString();
                u.Active = Convert.ToBoolean(dataReader["Active"]);

                users.Add(u);
            }

            return users;
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

    //Reads A User in the db that has the specific email
    public User ReadUserByEmail(string email)
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

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Email", email);

        cmd = CreateCommandWithStoredProcedureGeneral("spReadUserByEmail_MD_TB2", con, paramDic);

        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (dataReader.Read())
            {
                User u = new User();

                u.Id = Convert.ToInt32(dataReader["dbUserId"]);
                u.Name = dataReader["Name"].ToString();
                u.Email = dataReader["Email"].ToString();
                u.Password = dataReader["Password"].ToString();
                u.Active = Convert.ToBoolean(dataReader["Active"]);

                return u;
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

    //DELETES A User in the db by it userId
    public int DeleteUser(int userId)
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

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", userId);

        cmd = CreateCommandWithStoredProcedureGeneral("spDeleteUser_MD_TB2", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
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

    //UPDATES A User in the db by its userId
    public int UpdateUser(int userToUpdateId, User user)
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

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", userToUpdateId);
        paramDic.Add("@Name", user.Name);
        paramDic.Add("@Email", user.Email);
        paramDic.Add("@Password", user.Password);
        paramDic.Add("@Active", user.Active);

        cmd = CreateCommandWithStoredProcedureGeneral("spUpdateUser_MD_TB2", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
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

    ////ADDS A Game to a specific User's Collection
    ////-- Tags are inserted when the game is inserted into GamesTable.
    ////-- UsersGamesTable contains only UserId and GameId.
    ////-- Therefore,this method is adding an existing game to a user collection  and does not duplicate tags or inserting new tags.
    //public int AddGameToUserCollection(int userId, int gameId)
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@UserId", userId);
    //    paramDic.Add("@GameId", gameId);

    //    cmd = CreateCommandWithStoredProcedureGeneral("spAddGameToUserCollection_MD_TB2", con, paramDic);

    //    try
    //    {
    //        int numEffected = cmd.ExecuteNonQuery();
    //        return numEffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close();
    //        }
    //    }
    //}

    ////DELETES A Game from a specific User's Collection
    //public int DeleteGameFromUserCollection(int userId, int gameId)
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@UserId", userId);
    //    paramDic.Add("@GameId", gameId);

    //    cmd = CreateCommandWithStoredProcedureGeneral("spDeleteGameFromUserCollection_MD_TB2", con, paramDic);

    //    try
    //    {
    //        int numEffected = cmd.ExecuteNonQuery();
    //        return numEffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close();
    //        }
    //    }
    //}



    //RETURNS A LIST<Country> of a specific User's (his game Collection)
    public List<Country> GetUserCountries(int userId)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Country> countries = new List<Country>();

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@UserId", userId);

        cmd = CreateCommandWithStoredProcedureGeneral("spGetUserCountries_MD_TB2", con, paramDic);

        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                Country c = new Country();

                c.Id = Convert.ToInt32(dataReader["dbCountryId"]);
                c.Cca3 = dataReader["CCA3"].ToString();
                c.Name = dataReader["Name"].ToString();
                c.OfficialName = dataReader["OfficialName"].ToString();
                c.Capital = dataReader["Capital"].ToString();
                c.Region = dataReader["Region"].ToString();
                c.SubRegion = dataReader["SubRegion"].ToString();
                c.Population = Convert.ToInt64(dataReader["Population"]);
                c.Area = Convert.ToDouble(dataReader["Area"]);
                c.FlagUrl = dataReader["FlagUrl"].ToString();

                countries.Add(c);
            }

            return countries;
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






    ////Returns recommended games for a specific user according to his games tags
    //public List<Game> GetRecommendedCountries(int userId)
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;
    //    List<Game> games = new List<Game>();

    //    try
    //    {
    //        con = connect("myProjDB");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@UserId", userId);

    //    cmd = CreateCommandWithStoredProcedureGeneral("spGetRecommendedGames_MD_TB2", con, paramDic);

    //    try
    //    {
    //        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

    //        while (dataReader.Read())
    //        {
    //            Game g = new Game();

    //            g.Id = Convert.ToInt32(dataReader["dbGameId"]);
    //            g.SteamAppId = Convert.ToInt32(dataReader["SteamAppId"]);
    //            g.Name = dataReader["Name"].ToString();
    //            g.SteamUrl = dataReader["SteamUrl"].ToString();
    //            g.CapsuleImage = dataReader["CapsuleImage"].ToString();
    //            g.ReleaseDate = dataReader["ReleaseDate"].ToString();
    //            g.ReviewSummary = dataReader["ReviewSummary"].ToString();
    //            g.Price = Convert.ToInt32(dataReader["Price"]);
    //            g.Windows = Convert.ToBoolean(dataReader["Windows"]);
    //            g.Mac = Convert.ToBoolean(dataReader["Mac"]);
    //            g.Linux = Convert.ToBoolean(dataReader["Linux"]);

    //            games.Add(g);
    //        }

    //        dataReader.Close();

    //        // אחרי שסיימנו לקרוא וסגרנו reader ראשי - מביאים Tags
    //        for (int i = 0; i < games.Count; i++)
    //        {
    //            games[i].Tags = GetTagsByGameId(games[i].Id);
    //        }

    //        return games;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close();
    //        }
    //    }
    //}




}
