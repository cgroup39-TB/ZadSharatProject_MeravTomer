using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    /// <summary>
    /// Data access layer for Region reference data (the geographic regions countries belong to).
    /// </summary>
    public class DBRegionServices
    {
        /// <summary>
        /// Creates a new instance of the service. Holds no state; a fresh DB connection is opened per call.
        /// </summary>
        public DBRegionServices()
        {
        }


        /// <summary>
        /// Opens and returns a new <see cref="SqlConnection"/> using the named connection string
        /// read from appsettings.json. The caller is responsible for closing the connection.
        /// </summary>
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


        /// <summary>
        /// Returns every region in the database.
        /// </summary>
        public List<Region> ReadAllRegions()
        {
            SqlConnection con;
            SqlCommand cmd;
            List<Region> regions = new List<Region>();

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadAllRegions_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    Region region = new Region();

                    region.RegionId =
                        Convert.ToInt32(dataReader["RegionId"]);

                    region.RegionName =
                        dataReader["RegionName"].ToString();

                    regions.Add(region);
                }

                return regions;
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


        /// <summary>
        /// Returns the region matching <paramref name="regionId"/>, or null if none exists.
        /// </summary>
        public Region ReadRegionById(int regionId)
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

            paramDic.Add("@RegionId", regionId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadRegionById_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

                if (dataReader.Read())
                {
                    Region region = new Region();

                    region.RegionId =
                        Convert.ToInt32(dataReader["RegionId"]);

                    region.RegionName =
                        dataReader["RegionName"].ToString();

                    return region;
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


        /// <summary>
        /// Returns the region whose name matches <paramref name="regionName"/>, or null if none exists.
        /// </summary>
        public Region ReadRegionByName(string regionName)
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

            paramDic.Add("@RegionName", regionName);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadRegionByName_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

                if (dataReader.Read())
                {
                    Region region = new Region();

                    region.RegionId =
                        Convert.ToInt32(dataReader["RegionId"]);

                    region.RegionName =
                        dataReader["RegionName"].ToString();

                    return region;
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


        /// <summary>
        /// Inserts a new region using <paramref name="region"/>'s name.
        /// </summary>
        /// <returns>The database-generated RegionId of the newly inserted row.</returns>
        public int InsertRegion(Region region)
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
                "@RegionName",
                region.RegionName);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spInsertRegion_3MD_TB",
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