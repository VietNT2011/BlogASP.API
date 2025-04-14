using BlogASP.API.DTOs;
using BlogASP.API.Helpers;
using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogASP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasherHelper _passwordHasherHelper;

        public AuthController(IUserRepository userRepository, IConfiguration configuration, PasswordHasherHelper passwordHasherHelper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasherHelper = passwordHasherHelper;
        }

        // Register API
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO model)
        {
            // Check if the user already exists
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already taken.");
            }

            // Hash the password before saving to database
            var hashedPassword = _passwordHasherHelper.HashPassword(model.Password);

            // Create a new User
            var newUser = new User
            {
                UserName = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword
            };

            // Save the user to the database
            await _userRepository.CreateAsync(newUser);

            return Ok("User registered successfully.");
        }

        // Login API
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            // Retrieve the user by username
            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid Email or password.");
            }

            // Verify the password
            if (!_passwordHasherHelper.VerifyPassword(user.PasswordHash, model.Password))
            {
                return Unauthorized("Invalid Email or password.");
            }

            // Get JWT settings
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expirationMinutes = jwtSettings["ExpirationMinutes"]!;

            // Generate JWT token
            var token = JwtTokenHelper.GenerateJwtToken(_configuration, user.UserId, user.UserName);

            // Create the response DTO
            var response = new LoginResponseDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                AccessToken = token,
                ExpirationMinutes = expirationMinutes
            };

            return Ok(response);
        }
    }
}
