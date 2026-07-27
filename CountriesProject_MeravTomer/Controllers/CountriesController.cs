using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    /// <summary>
    /// REST endpoints for browsing, searching, importing and managing countries.
    /// Thin wrapper around <see cref="Country"/> (BL) - no business logic lives here.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        /// <summary>Returns every country.</summary>
        // GET: api/Countries
        [HttpGet]
        public IEnumerable<Country> Get()
        {
            Country country = new Country();
            return country.ReadAllCountries();
        }


        /// <summary>Returns the country matching <paramref name="id"/>, or 404 if none exists.</summary>
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


        /// <summary>Returns the country matching <paramref name="countryName"/> (exact match), or 404 if none exists.</summary>
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


        /// <summary>Returns all countries in the named region, or 404 if the region itself doesn't exist.</summary>
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


        /// <summary>Returns all countries sorted by "name" or "population" (400 for any other value); ascending unless <paramref name="ascending"/> is false.</summary>
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


        /// <summary>
        /// Currently a no-op: <see cref="Country.Insert"/> is a stub that does not persist to the
        /// database, so this endpoint always returns the posted object back unchanged rather than
        /// actually creating a country. Real country creation happens via <see cref="ImportCountries"/>.
        /// </summary>
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


        /// <summary>Updates the country identified by <paramref name="id"/>; returns 404 if it doesn't exist.</summary>
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


        /// <summary>Deletes the country identified by <paramref name="id"/>; returns 404 if it doesn't exist.</summary>
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


        /// <summary>
        /// Triggers a bulk import from the public countries.dev API, skipping countries that already
        /// exist. This is the actual way countries get created in this project (see note on <see cref="Post"/>).
        /// Any exception from the import is returned as 400 with the exception message.
        /// </summary>
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