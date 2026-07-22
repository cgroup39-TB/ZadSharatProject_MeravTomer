using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;
using ServerSideCountriesProject_MeravTomer.Models;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserBL userBL = new UserBL();

        [HttpPost("register")]
        public IActionResult Register([FromBody] User newUser)
        {
            try
            {
                return Ok(userBL.Register(newUser));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User loginCredentials)
        {
            try
            {
                User user = userBL.Login(loginCredentials?.Email, loginCredentials?.Password);
                return user == null ? Unauthorized("Invalid email or password") : Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            User user = userBL.GetUserById(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(int id, [FromBody] User updatedUser)
        {
            try
            {
                User result = userBL.UpdateProfile(id, updatedUser?.Name, updatedUser?.Email);
                return result == null ? NotFound() : Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/password")]
        public IActionResult ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                bool updated = userBL.ChangePassword(id, request?.CurrentPassword, request?.NewPassword);
                return updated ? Ok(new { message = "Password updated successfully" }) : NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/languages")]
        public IActionResult GetLanguages(int id)
        {
            List<UserLanguage> languages = userBL.GetUserLanguages(id);
            return languages == null ? NotFound() : Ok(languages);
        }

        [HttpPut("{id}/languages")]
        public IActionResult UpdateLanguages(int id, [FromBody] List<UserLanguage> languages)
        {
            try
            {
                bool updated = userBL.UpdateUserLanguages(id, languages);
                return updated ? Ok(new { message = "Languages updated successfully" }) : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/regions")]
        public IActionResult GetPreferredRegions(int id)
        {
            List<Region> regions = userBL.GetPreferredRegions(id);
            return regions == null ? NotFound() : Ok(regions);
        }

        [HttpPut("{id}/regions")]
        public IActionResult UpdatePreferredRegions(int id, [FromBody] List<int> regionIds)
        {
            bool updated = userBL.UpdatePreferredRegions(id, regionIds);
            return updated ? Ok(new { message = "Preferred regions updated successfully" }) : NotFound();
        }
    }
}
