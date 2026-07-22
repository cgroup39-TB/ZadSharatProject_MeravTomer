namespace ServerSideCountriesProject_MeravTomer.Models
{
    public class Region
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; }

        public Region()
        {
        }

        public Region(int regionId, string regionName)
        {
            RegionId = regionId;
            RegionName = regionName;
        }
    }
}
