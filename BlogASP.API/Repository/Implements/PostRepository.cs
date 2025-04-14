using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using BlogASP.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogASP.API.Repository.Implements
{
    // Implementation of the PostRepository with additional custom operations
    public class PostRepository(IOptions<MongoDbSettings> settings) : Repository<Post>(settings), IPostRepository
    {

        // Retrieves posts that belong to a specific category
        public async Task<IEnumerable<Post>> GetPostsByCategoryIdAsync(string categoryId)
        {
            // Filter to match posts containing the specified CategoryId in their Categories list
            var filter = Builders<Post>.Filter.ElemMatch(p => p.Categories, c => c.CategoryId == categoryId);

            // Fetch all posts matching the filter
            return await _collection.Find(filter).ToListAsync();
        }

        // Retrieves posts authored by a specific user
        public async Task<IEnumerable<Post>> GetPostsByAuthorIdAsync(string authorId)
        {
            // Filter to match posts with the specified AuthorId
            var filter = Builders<Post>.Filter.Eq(p => p.Author.UserId, authorId);

            // Fetch all posts matching the filter
            return await _collection.Find(filter).ToListAsync();
        }
    }
}
