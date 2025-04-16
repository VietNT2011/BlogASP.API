using BlogASP.API.Infrastructure.Cloundinary;
using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogASP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICloudinaryService _cloudinaryService;

        // Constructor injection
        public UserController(IUserRepository userRepository, ICloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
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

        // UploadAvatar
        [HttpPost("UploadAvatar/{userid}")]
        public async Task<IActionResult> UploadUserAvatarAsync(string userid, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }
            var user = await _userRepository.GetByIdAsync(userid);
            if (user == null)
            {
                return NotFound($"User with ID {userid} not found.");
            }

            try
            {
                // Upload imagen to cloundinary
                var uploadResult = await _cloudinaryService.UploadImageFromFileAsync(file);

                if (uploadResult?.SecureUrl == null)
                {
                    return StatusCode(500, "Failed to upload image: No secure URL returned.");
                }

                // Update link avatar into User
                user.AvatarURL = uploadResult.SecureUrl.ToString();
                await _userRepository.UpdateAsync(userid, user);

                return Ok(new { Message = "Avatar uploaded successfully", AvatarUrl = user.AvatarURL });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}

