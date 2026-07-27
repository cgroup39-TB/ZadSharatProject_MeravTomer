using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;
using System.Data.SqlClient;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    /// <summary>
    /// REST endpoints for a user's visited-country records (marking a country visited, rating/
    /// reviewing it, and sharing that review publicly). UserVisitedCountries has a composite
    /// (UserId, CountryId) primary key in the database, so duplicate-insert attempts throw a
    /// SqlException that <see cref="Post"/> catches and turns into 409 Conflict.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserVisitedCountriesController : ControllerBase
    {
        /// <summary>Returns every country the given user has marked visited (including unrated ones).</summary>
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


        /// <summary>Returns every visit record for the given country, across all users (not just shared ones).</summary>
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


        /// <summary>Returns every publicly shared review across all users and countries.</summary>
        // GET:
        // api/UserVisitedCountries/shared
        [HttpGet("shared")]
        public IActionResult GetAllShared()
        {
            List<UserVisitedCountry> result =
                UserVisitedCountry.ReadAllSharedReviews();

            return Ok(result);
        }


        /// <summary>Returns only the publicly shared reviews for the given country.</summary>
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


        /// <summary>Returns only the publicly shared reviews written by the given user.</summary>
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


        /// <summary>
        /// Marks a country as visited for a user. Accepts a partial <see cref="UserVisitedCountry"/>
        /// (only UserId + Country.CountryId are required, thanks to
        /// SuppressImplicitRequiredAttributeForNonNullableReferenceTypes in Program.cs). Because
        /// UserVisitedCountries has a composite (UserId, CountryId) primary key, marking the same
        /// country visited twice throws SqlException 2627/2601, caught here and returned as 409 Conflict.
        /// </summary>
        // POST:
        // api/UserVisitedCountries
        [HttpPost]
        public IActionResult Post(
            [FromBody] UserVisitedCountry visit)
        {
            try
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
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Conflict(
                    "This country is already in your list.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        /// <summary>Updates the rating/review text/share flag of an existing visit; returns 404 if it doesn't exist.</summary>
        // PUT:
        // api/UserVisitedCountries
        [HttpPut]
        public IActionResult Update(
            [FromBody] UserVisitedCountry visit)
        {
            try
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        /// <summary>Removes a user's visit record for the given country; returns 404 if it doesn't exist.</summary>
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