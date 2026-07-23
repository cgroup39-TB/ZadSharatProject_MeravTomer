using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // =========================
        // READ USERS
        // =========================

        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> Get()
        {
            User user = new User();

            return user.ReadAllUsers();
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            User user = new User();

            User result =
                user.ReadById(id);

            if (result == null)
            {
                return NotFound("User not found");
            }

            return Ok(result);
        }


        // GET: api/Users/getByName?name=May
        [HttpGet("getByName")]
        public IActionResult GetByName(string name)
        {
            User user = new User();

            User result =
                user.ReadByName(name);

            if (result == null)
            {
                return NotFound("User not found");
            }

            return Ok(result);
        }


        // =========================
        // REGISTER / LOGIN
        // =========================

        // POST: api/Users/register
        [HttpPost("register")]
        public IActionResult Register(
            [FromBody] User newUser)
        {
            try
            {
                User user = new User();

                int result =
                    user.Register(newUser);

                if (result == 0)
                {
                    return BadRequest(
                        "User was not registered");
                }

                return Ok(new
                {
                    message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: api/Users/login
        [HttpPost("login")]
        public IActionResult Login(
            [FromBody] User loginDetails)
        {
            try
            {
                User user = new User();

                User result =
                    user.Login(
                        loginDetails.Email,
                        loginDetails.Password);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // =========================
        // PROFILE
        // =========================

        // PUT:
        // api/Users/5/profile?actingUserId=5
        [HttpPut("{id}/profile")]
        public IActionResult UpdateProfile(
            int id,
            int actingUserId,
            [FromBody] User userDetails)
        {
            User reader = new User();

            User actingUser =
                reader.ReadById(actingUserId);

            if (actingUser == null)
            {
                return NotFound(
                    "Acting user not found");
            }

            try
            {
                int result =
                    actingUser.UpdateProfile(
                        id,
                        userDetails);

                if (result == 0)
                {
                    return NotFound(
                        "User not found");
                }

                return Ok(new
                {
                    message = "Profile updated successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // =========================
        // USER LANGUAGES
        // =========================

        // GET: api/Users/5/languages
        [HttpGet("{id}/languages")]
        public IActionResult GetUserLanguages(int id)
        {
            User user = new User();

            List<UserLanguages> result =
                user.ReadUserLanguages(id);

            return Ok(result);
        }


        // PUT: api/Users/5/languages
        [HttpPut("{id}/languages")]
        public IActionResult UpdateUserLanguages(
            int id,
            [FromBody] List<UserLanguages> languages)
        {
            User user = new User();

            user.UpdateUserLanguages(
                id,
                languages);

            return Ok(new
            {
                message = "User languages updated successfully"
            });
        }


        // =========================
        // USER REGIONS
        // =========================

        // GET: api/Users/5/regions
        [HttpGet("{id}/regions")]
        public IActionResult GetPreferredRegions(int id)
        {
            User user = new User();

            List<Region> result =
                user.ReadPreferredRegions(id);

            return Ok(result);
        }


        // PUT: api/Users/5/regions
        [HttpPut("{id}/regions")]
        public IActionResult UpdatePreferredRegions(
            int id,
            [FromBody] List<int> regionIds)
        {
            User user = new User();

            user.UpdatePreferredRegions(
                id,
                regionIds);

            return Ok(new
            {
                message =
                    "Preferred regions updated successfully"
            });
        }


        // =========================
        // WANTED COUNTRIES
        // =========================

        // GET: api/Users/5/wantedCountries
        [HttpGet("{id}/wantedCountries")]
        public IActionResult GetWantedCountries(int id)
        {
            User user = new User();

            List<Country> result =
                user.ReadWantedCountries(id);

            return Ok(result);
        }


        // POST: api/Users/5/wantedCountries/10
        [HttpPost("{id}/wantedCountries/{countryId}")]
        public IActionResult AddWantedCountry(
            int id,
            int countryId)
        {
            User user = new User();

            user.UserId = id;

            int result =
                user.AddWantedCountry(countryId);

            if (result == 0)
            {
                return BadRequest(
                    "Country was not added");
            }

            return Ok(new
            {
                message =
                    "Country added to wanted list"
            });
        }


        // DELETE:
        // api/Users/5/wantedCountries/10
        [HttpDelete("{id}/wantedCountries/{countryId}")]
        public IActionResult RemoveWantedCountry(
            int id,
            int countryId)
        {
            User user = new User();

            user.UserId = id;

            int result =
                user.RemoveWantedCountry(countryId);

            if (result == 0)
            {
                return NotFound(
                    "Country was not found in wanted list");
            }

            return Ok(new
            {
                message =
                    "Country removed from wanted list"
            });
        }


        // =========================
        // ADMIN
        // =========================

        // PUT:
        // api/Users/10/active?actingUserId=1
        [HttpPut("{targetUserId}/active")]
        public IActionResult SetUserActive(
            int targetUserId,
            int actingUserId,
            [FromBody] User userDetails)
        {
            User reader = new User();

            User admin =
                reader.ReadById(actingUserId);

            if (admin == null)
            {
                return NotFound(
                    "Acting user not found");
            }

            try
            {
                int result =
                    admin.SetUserActive(
                        targetUserId,
                        userDetails);

                if (result == 0)
                {
                    return NotFound(
                        "Target user not found");
                }

                return Ok(new
                {
                    message =
                        "User active status updated successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        // PUT:
        // api/Users/10/canShare?actingUserId=1
        [HttpPut("{targetUserId}/canShare")]
        public IActionResult SetCanShare(
            int targetUserId,
            int actingUserId,
            [FromBody] User userDetails)
        {
            User reader = new User();

            User admin =
                reader.ReadById(actingUserId);

            if (admin == null)
            {
                return NotFound(
                    "Acting user not found");
            }

            try
            {
                int result =
                    admin.SetCanShare(
                        targetUserId,
                        userDetails);

                if (result == 0)
                {
                    return NotFound(
                        "Target user not found");
                }

                return Ok(new
                {
                    message =
                        "Sharing permission updated successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        // PUT:
        // api/Users/10/admin?actingUserId=1
        [HttpPut("{targetUserId}/admin")]
        public IActionResult SetAdmin(
            int targetUserId,
            int actingUserId,
            [FromBody] User userDetails)
        {
            User reader = new User();

            User admin =
                reader.ReadById(actingUserId);

            if (admin == null)
            {
                return NotFound(
                    "Acting user not found");
            }

            try
            {
                int result =
                    admin.SetAdmin(
                        targetUserId,
                        userDetails);

                if (result == 0)
                {
                    return NotFound(
                        "Target user not found");
                }

                return Ok(new
                {
                    message =
                        "Admin permission updated successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        // =========================
        // ADMIN STATISTICS
        // =========================

        // GET:
        // api/Users/statistics?actingUserId=1
        [HttpGet("statistics")]
        public IActionResult GetStatistics(
            int actingUserId)
        {
            User reader = new User();

            User admin =
                reader.ReadById(actingUserId);

            if (admin == null)
            {
                return NotFound(
                    "User not found");
            }

            try
            {
                Dictionary<string, int> statistics =
                    admin.ReadStatistics();

                return Ok(statistics);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}