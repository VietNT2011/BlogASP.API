using BlogASP.API.Models;

namespace BlogASP.API.Repository.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(string postId);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(string userId);
    }
}
