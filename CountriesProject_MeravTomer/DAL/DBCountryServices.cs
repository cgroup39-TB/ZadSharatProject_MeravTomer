using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using CountriesProject_MeravTomer.BL;
using System.Xml.Linq;

namespace CountriesProject_MeravTomer.DAL
{
    public class DBCountryServices
    {

        public DBCountryServices()
        {
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



        //--------------------------------------------------------------------------------------------------
        // Returning a list of all games in the GamesTable
        //--------------------------------------------------------------------------------------------------

        public List<Country> ReadAllCountries()
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

            cmd = CreateCommandWithStoredProcedureGeneral("spReadAllGames_MD_TB2", con, null);

            try
            {
                SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    Country c = new Country();

                    c.Id = Convert.ToInt32(dataReader["dbCountryId"]);
                    c.Cca3 = dataReader["Cca3"].ToString();
                    c.Name = dataReader["Name"].ToString();
                    c.OfficialName = dataReader["OfficialName"].ToString();
                    c.Capital = dataReader["Capital"].ToString();
                    c.Region = dataReader["Region"].ToString();
                    c.SubRegion = dataReader["SubRegion"].ToString();
                    c.Population = Convert.ToInt32(dataReader["Population"]);
                    c.Area = Convert.ToInt32(dataReader["Population"]);
                    c.Latitude = Convert.ToBoolean(dataReader["Latitude"]);
                    c.Longitude = Convert.ToBoolean(dataReader["Longitude"]);
                    c.FlagUrl = Convert.ToBoolean(dataReader["FlagUrl"]);


                    //  Languages = new Dictionary<string, string>();
                    //   Currencies = new Dictionary<string, Currency>();
                    //Borders = new List<string> ;


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


        ////--------------------------------------------------------------------------------------------------
        //// This method reading a specific game by its gameId from the dataBase
        ////--------------------------------------------------------------------------------------------------
        //public Game ReadGameById(int id)
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
        //    paramDic.Add("@Id", id);

        //    cmd = CreateCommandWithStoredProcedureGeneral("spReadGameById_MD_TB2", con, paramDic);

        //    try
        //    {
        //        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //        if (dataReader.Read())
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
        //            g.Tags = GetTagsByGameId(g.Id);
        //            g.Windows = Convert.ToBoolean(dataReader["Windows"]);
        //            g.Mac = Convert.ToBoolean(dataReader["Mac"]);
        //            g.Linux = Convert.ToBoolean(dataReader["Linux"]);

        //            return g;
        //        }

        //        return null;
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



        ////--------------------------------------------------------------------------------------------------
        //// This method reading a specific game by its steamAppID from the dataBase
        ////--------------------------------------------------------------------------------------------------
        //public Game ReadGameBySteamAppId(int steamAppId)
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
        //    paramDic.Add("@SteamAppId", steamAppId);

        //    cmd = CreateCommandWithStoredProcedureGeneral(
        //        "spReadGameBySteamAppId_MD_TB2",
        //        con,
        //        paramDic);

        //    try
        //    {
        //        SqlDataReader dataReader = cmd.ExecuteReader();

        //        if (dataReader.Read())
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

        //            dataReader.Close();

        //            g.Tags = GetTagsByGameId(g.Id);

        //            return g;
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}

        ////--------------------------------------------------------------------------------------------------
        //// This method Reads all games containing a specific name from the GamesTable 
        ////--------------------------------------------------------------------------------------------------
        //public List<Game> ReadGamesByName(string gameName)
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
        //    paramDic.Add("@Name", gameName);

        //    cmd = CreateCommandWithStoredProcedureGeneral("spReadGamesByName_MD_TB2", con, paramDic);

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
        //            g.Tags = GetTagsByGameId(g.Id);
        //            g.Windows = Convert.ToBoolean(dataReader["Windows"]);
        //            g.Mac = Convert.ToBoolean(dataReader["Mac"]);
        //            g.Linux = Convert.ToBoolean(dataReader["Linux"]);

        //            games.Add(g);
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

        ////RETURNS A LIST<Game> of a specific User's (his game Collection)
        //public List<Game> GetUserGames(int userId)
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

        //    cmd = CreateCommandWithStoredProcedureGeneral("spGetUserGames_MD_TB2", con, paramDic);

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
        //            g.Tags = GetTagsByGameId(g.Id);
        //            g.Windows = Convert.ToBoolean(dataReader["Windows"]);
        //            g.Mac = Convert.ToBoolean(dataReader["Mac"]);
        //            g.Linux = Convert.ToBoolean(dataReader["Linux"]);

        //            games.Add(g);
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

        ////Returns recommended games for a specific user according to his games tags
        //public List<Game> GetRecommendedGames(int userId)
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

        ////--------------------------------------------------------------------------------------------------
        //// This method inserts a game to the GamesTable(GamesTable_MD_TB2) 
        ////--------------------------------------------------------------------------------------------------
        //public int InsertGame(Game game)
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
        //    paramDic.Add("@SteamAppId", game.SteamAppId);
        //    paramDic.Add("@Name", game.Name);
        //    paramDic.Add("@SteamUrl", game.SteamUrl);
        //    paramDic.Add("@CapsuleImage", game.CapsuleImage);
        //    paramDic.Add("@ReleaseDate", game.ReleaseDate);
        //    paramDic.Add("@ReviewSummary", game.ReviewSummary);
        //    paramDic.Add("@Price", game.Price);
        //    paramDic.Add("@Windows", game.Windows);
        //    paramDic.Add("@Mac", game.Mac);
        //    paramDic.Add("@Linux", game.Linux);

        //    cmd = CreateCommandWithStoredProcedureGeneral("spInsertGame_MD_TB2", con, paramDic);

        //    try
        //    {
        //        object result = cmd.ExecuteScalar();
        //        return Convert.ToInt32(result);
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


        ////--------------------------------------------------------------------------------------------------
        //// This method inserts 1 tag of a game into tagsTable(tagGameTable_MD_TB2) the gameId is supposed to be known when calling this function meaning it just inserts these rows it doesnt validate the row
        ////--------------------------------------------------------------------------------------------------
        //public int InsertGameTag(int gameId, string tagName)
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
        //    paramDic.Add("@GameId", gameId);
        //    paramDic.Add("@TagName", tagName);

        //    cmd = CreateCommandWithStoredProcedureGeneral("spInsertGameTag_MD_TB2", con, paramDic);

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

        ////--------------------------------------------------------------------------------------------------
        //// Updates a game in the gameTable (updating tags will do seperately)
        ////--------------------------------------------------------------------------------------------------
        //public int UpdateGame(int gameId, Game game)
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

        //    paramDic.Add("@ID", gameId);
        //    paramDic.Add("@SteamAppId", game.SteamAppId);
        //    paramDic.Add("@Name", game.Name);
        //    paramDic.Add("@SteamUrl", game.SteamUrl);
        //    paramDic.Add("@CapsuleImage", game.CapsuleImage);
        //    paramDic.Add("@ReleaseDate", game.ReleaseDate);
        //    paramDic.Add("@ReviewSummary", game.ReviewSummary);
        //    paramDic.Add("@Price", game.Price);
        //    paramDic.Add("@Windows", game.Windows);
        //    paramDic.Add("@Mac", game.Mac);
        //    paramDic.Add("@Linux", game.Linux);

        //    cmd = CreateCommandWithStoredProcedureGeneral("spUpdateGame_MD_TB2", con, paramDic);

        //    try
        //    {
        //        int numEffected = cmd.ExecuteNonQuery();
        //        if (numEffected > 0)
        //        {
        //            DeleteTagsByGameId(gameId);
        //            InsertManyTagsToGame(gameId, game.Tags);
        //        }

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

        ////--------------------------------------------------------------------------------------------------
        //// This method DELETES a game with a specific Id(Not SteamAppID)
        ////--------------------------------------------------------------------------------------------------
        //public int DeleteGame(int gameId)
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

        //    paramDic.Add("@Id", gameId);

        //    cmd = CreateCommandWithStoredProcedureGeneral("spDeleteGame_MD_TB2", con, paramDic);

        //    try
        //    {
        //        DeleteTagsByGameId(gameId);
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




        ////--------------------------------------------------------------------------------------------------
        //// This method Reads all Tags of Games
        ////--------------------------------------------------------------------------------------------------
        //public List<string> GetAllExistingTags()
        //{
        //    SqlConnection con;
        //    SqlCommand cmd;
        //    List<string> tags = new List<string>();

        //    try
        //    {
        //        con = connect("myProjDB");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    cmd = CreateCommandWithStoredProcedureGeneral("spGetAllTags_MD_TB2", con, null);

        //    try
        //    {
        //        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //        while (dataReader.Read())
        //        {
        //            tags.Add(dataReader["TagName"].ToString());
        //        }

        //        return tags;
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

        ////--------------------------------------------------------------------------------------------------
        //// get a string of tags and returns all the games that have one of these tags in the string
        ////--------------------------------------------------------------------------------------------------
        //public List<Game> GetGamesByTags(string tags)
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
        //    paramDic.Add("@Tags", tags);

        //    cmd = CreateCommandWithStoredProcedureGeneral("spGetGamesByTags_MD_TB2", con, paramDic);

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
        //            g.Tags = GetTagsByGameId(g.Id);
        //            g.Windows = Convert.ToBoolean(dataReader["Windows"]);
        //            g.Mac = Convert.ToBoolean(dataReader["Mac"]);
        //            g.Linux = Convert.ToBoolean(dataReader["Linux"]);

        //            games.Add(g);
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

        ////Get tags For a specific Game with gameId
        //public List<string> GetTagsByGameId(int gameId)
        //{
        //    SqlConnection con;
        //    SqlCommand cmd;
        //    List<string> tags = new List<string>();

        //    try
        //    {
        //        con = connect("myProjDB");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
        //    paramDic.Add("@GameId", gameId);

        //    cmd = CreateCommandWithStoredProcedureGeneral("spGetTagsByGameId_MD_TB2", con, paramDic);

        //    try
        //    {
        //        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //        while (dataReader.Read())
        //        {
        //            tags.Add(dataReader["TagName"].ToString());
        //        }

        //        return tags;
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

        ////DELETES tags For a specific Game with gameId
        //public int DeleteTagsByGameId(int gameId)
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

        //    paramDic.Add("@GameId", gameId);

        //    cmd = CreateCommandWithStoredProcedureGeneral(
        //        "spDeleteTagsByGameId_MD_TB2",
        //        con,
        //        paramDic);

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

        ////Inserts Many Tags in 1 method to a specific game in the tagGameTable
        //public int InsertManyTagsToGame(int gameId, List<string> tags)
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

        //    string tagsString = string.Join(",", tags);

        //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
        //    paramDic.Add("@GameId", gameId);
        //    paramDic.Add("@Tags", tagsString);

        //    cmd = CreateCommandWithStoredProcedureGeneral(
        //        "spInsertManyTagsToGame_MD_TB2",
        //        con,
        //        paramDic
        //    );

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
    }
}

