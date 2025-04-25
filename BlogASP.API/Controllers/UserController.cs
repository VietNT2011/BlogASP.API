using BlogASP.API.DTOs;
using BlogASP.API.Infrastructure.Cloundinary;
using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogASP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        // GET api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
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
        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.UserId))
            {
                return BadRequest("UserId is null.");
            }
            var existingUser = await _userRepository.GetByIdAsync(user.UserId);
            if (existingUser == null)
            {
                return NotFound($"User with ID {user.UserId} not found.");
            }

            await _userRepository.UpdateAsync(user.UserId, user);
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

        [HttpPost("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleToUserRequestDTO dto)
        {
            var result = await _userRepository.AddRoleToUserAsync(dto.UserId, dto.RoleId);
            if (!result)
            {
                return BadRequest("Failed to add role to user.");
            }

            return Ok("Role added successfully.");
        }

    }
}

