using ServerSideCountriesProject_MeravTomer.DAL;




namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>Business-logic representation of a geographic region (id + name).</summary>
    public class Region
    {
        private int regionId;
        private string regionName;


        /// <summary>Creates an empty region instance (used by model binding/deserialization).</summary>
        public Region()
        {
        }


        /// <summary>Creates a fully-populated region.</summary>
        public Region(int regionId, string regionName)
        {
            RegionId = regionId;
            RegionName = regionName;
        }


        public int RegionId
        {
            get => regionId;
            set => regionId = value;
        }


        public string RegionName
        {
            get => regionName;
            set => regionName = value;
        }


        // =========================
        // Read
        // =========================

        /// <summary>Returns every region in the database.</summary>
        public List<Region> ReadAllRegions()
        {
            DBRegionServices db = new DBRegionServices();

            return db.ReadAllRegions();
        }


        /// <summary>Returns the region matching <paramref name="regionId"/>, or null if none exists.</summary>
        public Region ReadRegionById(int regionId)
        {
            DBRegionServices db = new DBRegionServices();

            return db.ReadRegionById(regionId);
        }


        /// <summary>Returns the region matching <paramref name="regionName"/> (exact match), or null if none exists.</summary>
        public Region ReadRegionByName(string regionName)
        {
            DBRegionServices db = new DBRegionServices();

            return db.ReadRegionByName(regionName);
        }
    }
}
