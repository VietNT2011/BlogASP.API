namespace BlogASP.API.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Retrieves all documents from the collection
        Task<IEnumerable<T>> GetAllAsync();
        // Retrieves a single document by its ID
        Task<T?> GetByIdAsync(string id);
        // Inserts a new document into the collection
        Task CreateAsync(T entity);
        Task<T> CreateAndReturnEntityAsync(T entity);
        // Updates an existing document by its ID
        Task UpdateAsync(string id, T entity);
        // Deletes a document by its ID
        Task DeleteAsync(string id);
    }
}
