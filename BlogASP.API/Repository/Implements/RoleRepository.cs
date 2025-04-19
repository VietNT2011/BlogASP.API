using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using BlogASP.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogASP.API.Repository.Implements
{
    public class RoleRepository(IOptions<MongoDbSettings> settings) : Repository<Role>(settings), IRoleRepository
    {
        public async Task<Role?> GetRoleByIdAsync(string roleId)
        {
            var filter = Builders<Role>.Filter.Eq(r => r.RoleId, roleId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
