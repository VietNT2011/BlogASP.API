using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using BlogASP.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogASP.API.Repository.Implements
{
    public class UserRepository(IOptions<MongoDbSettings> settings) : Repository<User>(settings), IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _collection.Find(Builders<User>.Filter.Eq(u => u.Email, email)).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _collection.Find(Builders<User>.Filter.Eq(u => u.Username, username)).FirstOrDefaultAsync();
        }
    }
}
