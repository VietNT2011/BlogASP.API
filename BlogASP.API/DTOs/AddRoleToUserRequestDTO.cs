namespace BlogASP.API.DTOs
{
    public class AddRoleToUserRequestDTO
    {
        public string UserId { get; set; } = null!;
        public string RoleId { get; set; } = null!;
    }
}
