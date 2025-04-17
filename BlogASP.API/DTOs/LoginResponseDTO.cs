namespace BlogASP.API.DTOs
{
    public class LoginResponseDTO
    {
        public string? UserId { get; set; }
        public string? UserName {  get; set; }
        public string? AccessToken {get; set; }
        public string? AvatarURL { get; set; }
        public string? ExpirationMinutes {  get; set; }
    }
}
