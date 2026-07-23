using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserVisitedCountriesController : ControllerBase
    {
        // GET:
        // api/UserVisitedCountries/user/5
        [HttpGet("user/{userId}")]
        public IActionResult GetByUser(int userId)
        {
            List<UserVisitedCountry> result =
                UserVisitedCountry
                    .ReadVisitedCountriesByUser(userId);

            return Ok(result);
        }


        // GET:
        // api/UserVisitedCountries/country/10
        [HttpGet("country/{countryId}")]
        public IActionResult GetByCountry(
            int countryId)
        {
            List<UserVisitedCountry> result =
                UserVisitedCountry
                    .ReadVisitsByCountry(countryId);

            return Ok(result);
        }


        // GET:
        // api/UserVisitedCountries/shared/country/10
        [HttpGet("shared/country/{countryId}")]
        public IActionResult GetSharedByCountry(
            int countryId)
        {
            List<UserVisitedCountry> result =
                UserVisitedCountry
                    .ReadSharedReviewsByCountry(countryId);

            return Ok(result);
        }


        // GET:
        // api/UserVisitedCountries/shared/user/5
        [HttpGet("shared/user/{userId}")]
        public IActionResult GetSharedByUser(
            int userId)
        {
            List<UserVisitedCountry> result =
                UserVisitedCountry
                    .ReadSharedReviewsByUser(userId);

            return Ok(result);
        }


        // POST:
        // api/UserVisitedCountries
        [HttpPost]
        public IActionResult Post(
            [FromBody] UserVisitedCountry visit)
        {
            UserVisitedCountry result =
                visit.Insert();

            if (result == null)
            {
                return BadRequest(
                    "Visit was not inserted");
            }

            return Ok(result);
        }


        // PUT:
        // api/UserVisitedCountries
        [HttpPut]
        public IActionResult Update(
            [FromBody] UserVisitedCountry visit)
        {
            int result = visit.Update();

            if (result == 0)
            {
                return NotFound(
                    "Visit not found");
            }

            return Ok(new
            {
                message = "Visit updated successfully"
            });
        }


        // DELETE:
        // api/UserVisitedCountries/5/10
        [HttpDelete("{userId}/{countryId}")]
        public IActionResult Delete(
            int userId,
            int countryId)
        {
            UserVisitedCountry visit =
                new UserVisitedCountry();

            visit.UserId = userId;

            visit.Country = new Country();
            visit.Country.CountryId = countryId;

            bool result = visit.Delete();

            if (!result)
            {
                return NotFound(
                    "Visit not found");
            }

            return Ok(new
            {
                message = "Visit deleted successfully"
            });
        }
    }
}