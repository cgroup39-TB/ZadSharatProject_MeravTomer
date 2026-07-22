using ServerSideCountriesProject_MeravTomer.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {

        // GET: api/< CountriesController> get all countries

        [HttpGet]
        public IEnumerable<Country> Get()
        {
            Country country = new Country();
            return country.ReadAllCountries();
        }
        //GET1 BY CountryID A country FROM DATABASE
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

        //GET2 BY region A GAME FROM DATABASE
        [HttpGet("getByRegion/{region}")]
        public IActionResult GetByRegion(string region)
        {
            Country country = new Country();

            List <Country> result = country.ReadCountryByRegion(region);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        // GET3: api/<CountriesController> #Query String Version1?-- this is QueryString the parameter comes through the headline of the website URL
        [HttpGet("getByName")]
        public IEnumerable<Country> GetByName(string countryName)
        {
            Country country = new Country();
            return (IEnumerable<Country>)country.ReadCountryByName(countryName);
        }

        //// GET3.2  api/<CountriesController> #Query String Version2? what is the difference between those 2?-- this is RouteParameter Way meaning its part of the api URL
        //[HttpGet("getByNameR/{countryName}")]
        //public IEnumerable<Country> GetByNameR(string gameName)
        //{
        //    Country game = new Country();
        //    return game.ReadByContainName(gameName);
        //}



        [HttpPost]
        public IActionResult Post([FromBody] Country country)
        {
            Country insertedCountry = country.Insert();

            if (insertedCountry == null)
            {
                return BadRequest("Game was not inserted");
            }

            return Ok(insertedCountry);
        }

        // PUT api/<CountriesController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateCountry(int id, [FromBody] Country updatedCountry)
        {
            Country country = new Country();
            int result = country.UpdateCountry(id, updatedCountry);

            if (result == 0)
            {
                return NotFound();  
            }

            return Ok(new { message = "Country updated successfully" });
        }

        // DELETE api/<CountriesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Country country = new Country();
            int result = country.Delete(id);

            if (result == 0)
            {
                return NotFound(new { message = "Country was not found" });
            }

            return Ok(new { message = "Country was deleted successfully" });
        }

    }
}
