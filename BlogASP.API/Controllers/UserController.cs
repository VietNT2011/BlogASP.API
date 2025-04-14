using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogASP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        // Constructor injection
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //GET api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        // GET api/user/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} not found.");
            }

            return Ok(user);
        }

        // GET api/user/username/{username}
        [HttpGet("username/{username}")]
        public async Task<ActionResult<User>> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return NotFound($"User with username {username} not found.");
            }

            return Ok(user);
        }

        // POST api/user
        [HttpPost]
        public async Task<ActionResult<User>> CreateUserAsync([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User data is null.");
            }

            // Optionally, check if user already exists based on email or username
            var existingUserByEmail = await _userRepository.GetByEmailAsync(user.Email!);
            if (existingUserByEmail != null)
            {
                return Conflict("User with this email already exists.");
            }

            var existingUserByUsername = await _userRepository.GetByUsernameAsync(user.UserName!);
            if (existingUserByUsername != null)
            {
                return Conflict("User with this username already exists.");
            }

            await _userRepository.CreateAsync(user);
            return CreatedAtAction(nameof(GetUserByEmailAsync), new { email = user.Email }, user);
        }

        // PUT api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(string id, [FromBody] User user)
        {
            if (user == null || user.UserId != id)
            {
                return BadRequest("User data is invalid.");
            }

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            await _userRepository.UpdateAsync(id, user);
            return NoContent();
        }

        // DELETE api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}

