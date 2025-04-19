using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using BlogASP.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime;

namespace BlogASP.API.Repository.Implements
{
    public class UserRepository(IOptions<MongoDbSettings> settings, IRoleRepository roleRepository) : Repository<User>(settings), IUserRepository
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _collection.Find(Builders<User>.Filter.Eq(u => u.Email, email)).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _collection.Find(Builders<User>.Filter.Eq(u => u.UserName, username)).FirstOrDefaultAsync();
        }
        public async Task<bool> AddRoleToUserAsync(string userId, string roleId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);
            var user = await _collection.Find(filter).FirstOrDefaultAsync();

            if (user == null) return false;

            var role = await _roleRepository.GetRoleByIdAsync(roleId);

            if (role == null) return false;

            if (user.Role == null) user.Role = new List<Role>();

            if (!user.Role.Any(r => r.RoleId == roleId))
            {
                user.Role.Add(role);
            }
            else
            {
                return false;
            }
            var update = Builders<User>.Update.Set(u => u.Role, user.Role);
            var result = await _collection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

    }
}
