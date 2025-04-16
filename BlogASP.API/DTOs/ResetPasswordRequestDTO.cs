namespace BlogASP.API.DTOs
{
    public class ResetPasswordRequestDTO
    {
        public string? Email { get; set; }
        public string? PasswordResetToken { get; set; }
        public string? NewPassword {  get; set; }
    }
}
