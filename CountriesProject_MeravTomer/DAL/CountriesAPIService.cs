using CountriesProject_MeravTomer.BL;
using ServerSideCountriesProject_MeravTomer.DAL.dto;
using System.Diagnostics.Metrics;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    public class CountriesAPIService
    {
        private const string BaseUrl = "https://restcountries.com/v3.1";
        private const string Fields = "cca3,name,capital,region,subregion,population,area,latlng,flags,languages,currencies,borders";

        private readonly HttpClient httpClient;

        public CountriesAPIService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        public async Task<List<Country>> FetchAllCountriesFromApiAsync() {

            string url = $"{BaseUrl}/all?fields={Fields}";

            List<CountryAPIDto>? dtos = await httpClient.GetFromJsonAsync<List<CountryAPIDto>>(url);

            List<Country> countries = new List<Country>();

            if (dtos == null)
            {
                return countries;
            }

            foreach (CountryAPIDto dto in dtos)
            {
                countries.Add(MapApiDtoToCountry(dto));
            }

            return countries;

        }


        public async Task<Country?> FetchCountryByCca3FromApiAsync(string cca3) {

            string url = $"{BaseUrl}/alpha/{cca3}?fields={Fields}";

            CountryAPIDto dto = await httpClient.GetFromJsonAsync<CountryAPIDto>(url);

            if (dto == null)
            {
                return null;
            }

            return MapApiDtoToCountry(dto);
        }


        private Country MapApiDtoToCountry(CountryAPIDto dto) {

            Country country = new Country();

            country.Cca3 = dto.Cca3;
            country.Name = dto.Name?.Common ?? string.Empty;
            country.OfficialName = dto.Name?.Official ?? string.Empty;
            country.Capital = dto.Capital ?? new List<string>();
            country.Region = dto.Region;
            country.SubRegion = dto.Subregion;
            country.Population = dto.Population;
            country.Area = dto.Area;
            country.Latitude = dto.Latlng != null && dto.Latlng.Count > 0 ? dto.Latlng[0] : 0;
            country.Longitude = dto.Latlng != null && dto.Latlng.Count > 1 ? dto.Latlng[1] : 0;
            country.FlagUrl = dto.Flags?.Png ?? string.Empty;
            country.Languages = dto.Languages ?? new Dictionary<string, string>();
            country.Borders = dto.Borders ?? new List<string>();
            country.Currencies = new Dictionary<string, Currency>();
           
            if (dto.Currencies != null)
            {
                foreach (KeyValuePair<string, CountryCurrencyDto> entry in dto.Currencies)
                {
                    Currency currency = new Currency();
                    currency.Name = entry.Value.Name;
                    currency.Symbol = entry.Value.Symbol;
                    country.Currencies[entry.Key] = currency;
                }
            }

            return country;

        }
    }
}
