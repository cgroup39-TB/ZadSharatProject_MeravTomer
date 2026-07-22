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

        // GET: CountriesController

        [HttpGet]
        public IEnumerable<Country> Get()
        {
            Country country = new Country();
            return country.Read();
        }
        //GET1 BY dbGameID A GAME SO EDIT IN CLIEND SIDE
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Country country = new Country();
            Country result = country.ReadById(id);

            if (result == null)
            {
                return NotFound("Country not found");
            }

            return Ok(result);
        }

        //GET2 BY steamAppId A GAME FROM DATABASE
        [HttpGet("getBySteamAppId/{steamAppId}")]
        public IActionResult GetByRegion(string region)
        {
            Country country = new Country();

            List <Country> result = country.ReadByRegion(region);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        // GET3: api/<GamesController> #Query String Version1?-- this is QueryString the parameter comes through the headline of the website URL
        [HttpGet("getByName")]
        public IEnumerable<Country> GetByName(string countryName)
        {
            Country country = new Country();
            return (IEnumerable<Country>)country.ReadByName(countryName);
        }

        // GET3.2  api/<GamesController> #Query String Version2? what is the difference between those 2?-- this is RouteParameter Way meaning its part of the api URL
        [HttpGet("getByNameR/{gameName}")]
        public IEnumerable<Country> GetByNameR(string gameName)
        {
            Country game = new Country();
            return game.ReadByContainName(gameName);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Country game)
        {
            Country insertedGame = game.Insert();

            if (insertedGame == null)
            {
                return BadRequest("Game was not inserted");
            }

            return Ok(insertedGame);
        }

        // PUT api/<GamesController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateGame(int id, [FromBody] Country updatedCountry)
        {
            Country country = new Country();
            int result = country.UpdateCountry(id, updatedCountry);

            if (result == 0)
            {
                return NotFound();
            }

            return Ok(new { message = "Country updated successfully" });
        }

        // DELETE api/<GamesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Country game = new Country();
            int result = game.Delete(id);

            if (result == 0)
            {
                return NotFound(new { message = "Game was not found" });
            }

            return Ok(new { message = "Game was deleted successfully" });
        }

        // GET: api/<GamesController> -gets all the tags that are existing 
        [HttpGet("getAllTags")]
        public IActionResult GetAllTags()
        {
            try
            {
                Country game = new Country();
                return Ok(game.GetAllExistingTags());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // GET: api/<GamesController> -gets all the games that are having one of the tags in the string
        [HttpGet("getByTags")]
        public IActionResult GetByTags(string tags)
        {
            try
            {
                Country game = new Country();
                return Ok(game.GetGamesByTags(tags));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        private readonly CountriesAPIService _countriesService;

        public CountriesController(CountriesAPIService countriesService) // <-- כאן ה-DI מזריק
        {
            _countriesService = countriesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _countriesService.GetCountriesAsync();
            return Ok(countries);
        }


        ////POST api/<GamesController>/
        //[HttpPost("oneTimeLoadData")]
        //public IActionResult OneTimeLoadData([FromBody] List<Game> games)
        //{
        //    try
        //    {
        //        return Ok(Country.OneTimeLoadData(games));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
