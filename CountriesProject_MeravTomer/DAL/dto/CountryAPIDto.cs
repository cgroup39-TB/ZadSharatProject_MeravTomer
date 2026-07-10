using System.Text.Json.Serialization;

namespace ServerSideCountriesProject_MeravTomer.DAL.dto
{
    public class CountryAPIDto
    {
        [JsonPropertyName("cca3")]
        public string Cca3 { get; set; }

        [JsonPropertyName("name")]
        public CountryNameDto Name { get; set; }

        [JsonPropertyName("capital")]
        public List<string> Capital { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("subregion")]
        public string Subregion { get; set; }

        [JsonPropertyName("population")]
        public long Population { get; set; }

        [JsonPropertyName("area")]
        public double Area { get; set; }

        [JsonPropertyName("latlng")]
        public List<double> Latlng { get; set; }

        [JsonPropertyName("flags")]
        public CountryFlagsDto Flags { get; set; }

        [JsonPropertyName("languages")]
        public Dictionary<string, string> Languages { get; set; }

        [JsonPropertyName("currencies")]
        public Dictionary<string, CountryCurrencyDto> Currencies { get; set; }

        [JsonPropertyName("borders")]
        public List<string> Borders { get; set; }
    }

    public class CountryNameDto
    {
        [JsonPropertyName("common")]
        public string Common { get; set; }

        [JsonPropertyName("official")]
        public string Official { get; set; }
    }

    public class CountryFlagsDto
    {
        [JsonPropertyName("png")]
        public string Png { get; set; }

        [JsonPropertyName("svg")]
        public string Svg { get; set; }
    }

    public class CountryCurrencyDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }
}

