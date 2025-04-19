using BlogASP.API.Models;

namespace BlogASP.API.Repository.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetRoleByIdAsync(string roleId);
    }
}
