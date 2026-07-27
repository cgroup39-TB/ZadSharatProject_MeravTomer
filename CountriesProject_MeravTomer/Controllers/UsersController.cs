using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;
using System.Data.SqlClient;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    /// <summary>
    /// REST endpoints for users: read/register/login, profile editing, languages/preferred
    /// regions, the wanted-countries wishlist, and admin-only operations (activate/deactivate,
    /// grant sharing, grant admin, statistics). Authorization checks are enforced in the BL
    /// layer (<see cref="User"/>) rather than here, so most admin endpoints look up an "acting
    /// user" by id and rely on it to throw <see cref="UnauthorizedAccessException"/> if not an admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // =========================
        // READ USERS
        // =========================

        /// <summary>Returns every user (admin user-management list).</summary>
        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> Get()
        {
            User user = new User();

            return user.ReadAllUsers();
        }


        /// <summary>Returns the user matching <paramref name="id"/>, or 404 if none exists.</summary>
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


        /// <summary>Returns the user matching <paramref name="name"/>, or 404 if none exists.</summary>
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

        /// <summary>
        /// Registers a new user. Forces IsActive=true, IsAdmin=false, CanShare=true regardless of
        /// what was posted. Returns 400 (with the exception message) if the email is already taken.
        /// </summary>
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


        /// <summary>
        /// Authenticates by email/password. Returns 401 if the account has been deactivated
        /// (blocked) by an admin, and 400 for an unknown email, wrong password, or a missing body.
        /// On success returns the full <see cref="User"/> object (used as the client's session state).
        /// </summary>
        // POST: api/Users/login
        [HttpPost("login")]
        public IActionResult Login(
            [FromBody] LoginRequest loginDetails)
        {
            try
            {
                if (loginDetails == null)
                {
                    return BadRequest("Email and password are required.");
                }

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

        /// <summary>
        /// Updates a user's name/email. <paramref name="actingUserId"/> must either be the same
        /// user or an admin (enforced in <see cref="User.UpdateProfile"/>); returns 401 otherwise,
        /// 404 if the acting user or target user doesn't exist.
        /// </summary>
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

        /// <summary>Returns the languages (and proficiency levels) recorded for the given user.</summary>
        // GET: api/Users/5/languages
        [HttpGet("{id}/languages")]
        public IActionResult GetUserLanguages(int id)
        {
            User user = new User();

            List<UserLanguages> result =
                user.ReadUserLanguages(id);

            return Ok(result);
        }


        /// <summary>Replaces the user's recorded languages with the posted list.</summary>
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

        /// <summary>Returns the regions the given user has marked as a travel preference.</summary>
        // GET: api/Users/5/regions
        [HttpGet("{id}/regions")]
        public IActionResult GetPreferredRegions(int id)
        {
            User user = new User();

            List<Region> result =
                user.ReadPreferredRegions(id);

            return Ok(result);
        }


        /// <summary>Replaces the user's preferred-region list with the posted region ids.</summary>
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

        /// <summary>Returns the countries on the given user's wishlist ("wanted" countries).</summary>
        // GET: api/Users/5/wantedCountries
        [HttpGet("{id}/wantedCountries")]
        public IActionResult GetWantedCountries(int id)
        {
            User user = new User();

            List<Country> result =
                user.ReadWantedCountries(id);

            return Ok(result);
        }


        /// <summary>
        /// Adds a country to the user's wishlist. UserWantedCountries has a composite (UserId,
        /// CountryId) primary key, so adding a country already on the wishlist throws SqlException
        /// 2627/2601, caught here and returned as 409 Conflict.
        /// </summary>
        // POST: api/Users/5/wantedCountries/10
        [HttpPost("{id}/wantedCountries/{countryId}")]
        public IActionResult AddWantedCountry(
            int id,
            int countryId)
        {
            try
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
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Conflict(
                    "This country is already in your wishlist.");
            }
        }


        /// <summary>Removes a country from the user's wishlist; returns 404 if it wasn't on the list.</summary>
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

        /// <summary>
        /// Activates/deactivates (blocks) <paramref name="targetUserId"/>. <paramref name="actingUserId"/>
        /// must be an admin - returns 401 if not, 404 if either user doesn't exist.
        /// </summary>
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


        /// <summary>
        /// Grants/revokes <paramref name="targetUserId"/>'s permission to share reviews.
        /// <paramref name="actingUserId"/> must be an admin - returns 401 if not, 404 if either user doesn't exist.
        /// </summary>
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


        /// <summary>
        /// Grants/revokes admin rights for <paramref name="targetUserId"/>. <paramref name="actingUserId"/>
        /// must itself already be an admin - returns 401 if not, 404 if either user doesn't exist.
        /// </summary>
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


        /// <summary>
        /// Permanently deletes <paramref name="targetUserId"/>. <paramref name="actingUserId"/>
        /// must be an admin and cannot delete their own account - returns 401 in either case,
        /// 404 if either user doesn't exist.
        /// </summary>
        // DELETE:
        // api/Users/10?actingUserId=1
        [HttpDelete("{targetUserId}")]
        public IActionResult DeleteUser(
            int targetUserId,
            int actingUserId)
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
                    admin.DeleteUser(targetUserId);

                if (result == 0)
                {
                    return NotFound(
                        "Target user not found");
                }

                return Ok(new
                {
                    message =
                        "User deleted successfully"
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

        /// <summary>
        /// Returns admin dashboard counters (daily logins, imported/saved countries, shared
        /// reviews). <paramref name="actingUserId"/> must be an admin - returns 401 if not, 404 if that user doesn't exist.
        /// </summary>
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
                AdminStatistics statistics =
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