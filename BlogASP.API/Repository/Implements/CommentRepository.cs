using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using BlogASP.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogASP.API.Repository.Implements
{
    public class CommentRepository(IOptions<MongoDbSettings> settings) : Repository<Comment>(settings), ICommentRepository
    {
        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(string postId)
        {
            var filter = Builders<Comment>.Filter.Eq(p => p.PostId, postId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(string userId)
        {
            var filter = Builders<Comment>.Filter.Eq(p => p.Author.UserId, userId);
            return await _collection.Find(filter).ToListAsync();
        }
    }
}
