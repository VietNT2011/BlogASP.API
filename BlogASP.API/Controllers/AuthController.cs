using BlogASP.API.DTOs;
using BlogASP.API.Helpers;
using BlogASP.API.Infrastructure.EmailService;
using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ZstdSharp.Unsafe;

namespace BlogASP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUserRepository userRepository, IConfiguration configuration, PasswordHasherHelper passwordHasherHelper, IEmailService emailService) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;
        private readonly PasswordHasherHelper _passwordHasherHelper = passwordHasherHelper;
        private readonly IEmailService _emailService = emailService;

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
            if (!_passwordHasherHelper.VerifyPassword(user.PasswordHash!, model.Password))
            {
                return Unauthorized("Invalid Email or password.");
            }

            // Get JWT settings
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expirationMinutes = jwtSettings["ExpirationMinutes"]!;

            // Generate JWT token
            var token = JwtTokenHelper.GenerateJwtToken(_configuration, user);

            // Create the response DTO
            var response = new LoginResponseDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                AvatarURL = user.AvatarURL,
                AccessToken = token,
                ExpirationMinutes = expirationMinutes
            };

            return Ok(response);
        }
        //ForgotPassword API
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User with this email does not exist.");
            }

            // Generate a random token for password reset
            var random = new Random();
            var resetToken = random.Next(10000000, 99999999).ToString();
            var tokenExpiry = DateTime.UtcNow.AddMinutes(15); // Set token expiration time (e.g., 15 minutes)

            // Save the token and its expiry time to the user
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = tokenExpiry;
            await _userRepository.UpdateAsync(user.UserId, user);

            // Send email with the reset token
            // Email subject
            var emailSubject = "Password Reset Request";
            // Email body
            var emailBody = $@"
            <p>Here is your <strong>password reset token</strong>: <strong>{resetToken}</strong>.</p>
            <p>It will expire in <strong>15 minutes</strong>.</p>
            ";
            await _emailService.SendEmailAsync(email, emailSubject, emailBody);

            return Ok("Password reset token has been sent to your email.");
        }

        //ResetPassword API
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO model)
        {
            var user = await _userRepository.GetByEmailAsync(model.Email!);
            if (user == null)
            {
                return BadRequest("User with this email does not exist.");
            }

            // Check if the token is valid and has not expired
            if (user.PasswordResetToken != model.PasswordResetToken || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired token.");
            }

            // Hash the new password
            var hashedPassword = _passwordHasherHelper.HashPassword(model.NewPassword!);

            // Update the user's password
            user.PasswordHash = hashedPassword;

            // Clear the reset token after use
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _userRepository.UpdateAsync(user.UserId,user);

            return Ok("Password has been reset successfully.");
        }
        //ChangePassword API
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDTO model)
        {
            if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest("Invalid request. Please provide all required fields.");
            }

            var user = await _userRepository.GetByEmailAsync(model.Email!);
            if (user == null)
            {
                return BadRequest("User with this email does not exist.");
            }

            // Verify old password
            if (!_passwordHasherHelper.VerifyPassword(user.PasswordHash!, model.OldPassword))
            {
                return BadRequest("The old password is incorrect.");
            }

            // Hash the new password
            var hashedPassword = _passwordHasherHelper.HashPassword(model.NewPassword);

            // Update user's password
            user.PasswordHash = hashedPassword;
            await _userRepository.UpdateAsync(user.UserId, user);

            return Ok("Password has been successfully changed.");
        }
    }
}
