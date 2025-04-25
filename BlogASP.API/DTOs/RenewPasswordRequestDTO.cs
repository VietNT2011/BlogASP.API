namespace BlogASP.API.DTOs
{
    public class RenewPasswordRequestDTO
    {
        public string? UserId {  get; set; }
        public string? NewPassword {  get; set; }
    }
}
