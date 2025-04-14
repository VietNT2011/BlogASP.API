using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using BlogASP.API.Settings;
using Microsoft.Extensions.Options;

namespace BlogASP.API.Repository.Implements
{
    public class CategoryRepository(IOptions<MongoDbSettings> settings) : Repository<Category>(settings), ICategoryRepository
    {
    }
}
