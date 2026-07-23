using ServerSideCountriesProject_MeravTomer.DAL;




namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Region
    {
        private int regionId;
        private string regionName;


        public Region()
        {
        }


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

        public List<Region> ReadAllRegions()
        {
            DBRegionServices db = new DBRegionServices();

            return db.ReadAllRegions();
        }


        public Region ReadRegionById(int regionId)
        {
            DBRegionServices db = new DBRegionServices();

            return db.ReadRegionById(regionId);
        }


        public Region ReadRegionByName(string regionName)
        {
            DBRegionServices db = new DBRegionServices();

            return db.ReadRegionByName(regionName);
        }
    }
}
