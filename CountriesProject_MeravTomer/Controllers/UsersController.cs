using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;
using System.Diagnostics.Metrics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            User user = new User();

            return user.ReadAllUsers();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            User user = new User();
            User result = user.ReadById(id);

            if (result == null)
            {
                return NotFound("User not found");
            }

            return Ok(result);
        }

        [HttpGet("getByName")]
        public IActionResult GetByName(string name)
        {
            User user = new User();
            User result = user.ReadByName(name);

            if (result == null)
            {
                return NotFound("User not found");
            }

            return Ok(result);
        }


        [HttpGet("getUserLanguages")]
        public IActionResult GetUserLanguages(int id)
        {
            User user = new User();
            User result = user.ReadUserLanguages(id);

            if (result == null)
            {
                return NotFound("User not found");
            }

            return Ok(result);
        }


        // POST api/<UsersController>
        [HttpPost]

     //   public void Post([FromBody] User updateUser)
        public IActionResult UpdateUser([FromBody] User updateUser)
        {
            User user = new User();
            int result=user.UpdateProfile(updateUser);
            if (result == 0) {

                NotFound("User not found");
            }

            return Ok(new { message = "User updated successfully" });

        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
