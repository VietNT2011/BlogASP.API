namespace BlogASP.API.DTOs
{
    public class ChangePasswordRequestDTO
    {
        public string? Email { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
