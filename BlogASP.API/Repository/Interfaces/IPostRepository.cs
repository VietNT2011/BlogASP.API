using BlogASP.API.Models;

namespace BlogASP.API.Repository.Interfaces
{
    // Interface for Post-specific repository operations
    public interface IPostRepository : IRepository<Post>
    {
        // Retrieves a list of posts by a specific category ID
        Task<IEnumerable<Post>> GetPostsByCategoryIdAsync(string categoryId);

        // Retrieves a list of posts by a specific author ID
        Task<IEnumerable<Post>> GetPostsByAuthorIdAsync(string authorId);
    }
}
