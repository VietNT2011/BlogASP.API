using BlogASP.API.Models;

namespace BlogASP.API.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> AddRoleToUserAsync(string userId, string roleId);
    }
}
