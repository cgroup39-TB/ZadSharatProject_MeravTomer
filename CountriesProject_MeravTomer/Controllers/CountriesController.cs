using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        // GET: api/Countries
        [HttpGet]
        public IEnumerable<Country> Get()
        {
            Country country = new Country();
            return country.ReadAllCountries();
        }


        // GET: api/Countries/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Country country = new Country();
            Country result = country.ReadCountryById(id);

            if (result == null)
            {
                return NotFound("Country not found");
            }

            return Ok(result);
        }


        // GET: api/Countries/getByName?countryName=Israel
        [HttpGet("getByName")]
        public IActionResult GetByName(string countryName)
        {
            Country country = new Country();
            Country result = country.ReadCountryByName(countryName);

            if (result == null)
            {
                return NotFound("Country not found");
            }

            return Ok(result);
        }


        // GET: api/Countries/getByRegion/Europe
        [HttpGet("getByRegion/{regionName}")]
        public IActionResult GetByRegion(string regionName)
        {
            Region region = new Region();
            Region selectedRegion = region.ReadRegionByName(regionName);

            if (selectedRegion == null)
            {
                return NotFound("Region not found");
            }

            Country country = new Country();

            List<Country> result =
                country.ReadCountriesByRegion(selectedRegion);

            return Ok(result);
        }


        // GET: api/Countries/sort?sortBy=name&ascending=true
        // GET: api/Countries/sort?sortBy=population&ascending=false
        [HttpGet("sort")]
        public IActionResult GetSortedCountries(
            string sortBy,
            bool ascending = true)
        {
            if (sortBy != "name" &&
                sortBy != "population")
            {
                return BadRequest(
                    "sortBy must be 'name' or 'population'");
            }

            Country country = new Country();

            List<Country> result =
                country.ReadSortedCountries(
                    sortBy,
                    ascending);

            return Ok(result);
        }


        // POST: api/Countries
        [HttpPost]
        public IActionResult Post(
            [FromBody] Country country)
        {
            Country insertedCountry = country.Insert();

            if (insertedCountry == null)
            {
                return BadRequest(
                    "Country was not inserted");
            }

            return Ok(insertedCountry);
        }


        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public IActionResult UpdateCountry(
            int id,
            [FromBody] Country updatedCountry)
        {
            Country country = new Country();

            int result =
                country.UpdateCountry(
                    id,
                    updatedCountry);

            if (result == 0)
            {
                return NotFound(
                    "Country not found");
            }

            return Ok(new
            {
                message = "Country updated successfully"
            });
        }


        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Country country = new Country();

            int result =
                country.DeleteCountry(id);

            if (result == 0)
            {
                return NotFound(new
                {
                    message = "Country was not found"
                });
            }

            return Ok(new
            {
                message = "Country deleted successfully"
            });
        }


        // POST: api/Countries/import
        [HttpPost("import")]
        public async Task<IActionResult> ImportCountries()
        {
            try
            {
                Country country = new Country();

                int inserted =
                    await country.ImportCountriesFromApi();

                return Ok(new
                {
                    message = "Countries imported successfully",
                    insertedCountries = inserted
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}