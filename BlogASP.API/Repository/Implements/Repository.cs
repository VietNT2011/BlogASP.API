using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using BlogASP.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlogASP.API.Repository.Implements
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly IMongoCollection<T> _collection;
        public Repository(IOptions<MongoDbSettings> settings)
        {
            try
            {
                // Instance of Mongo Client
                var client = new MongoClient(settings.Value.ConnectionString);

                // Database instance
                var database = client.GetDatabase(settings.Value.DatabaseName);

                // Collection from the database
                _collection = database.GetCollection<T>(typeof(T).Name);

                Console.WriteLine("Connected to MongoDB successfully.");
            }
            catch (MongoConfigurationException ex)
            {
                Console.WriteLine($"MongoDB Configuration Error: {ex.Message}");
                throw;
            }
            catch (MongoConnectionException ex)
            {
                Console.WriteLine($"MongoDB Connection Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }


        // Retrieves all documents from the collection
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Find all documents and convert the result to a list
            return await _collection.Find(_ => true).ToListAsync();
        }

        // Retrieves a single document by its ID
        public async Task<T?> GetByIdAsync(string id)
        {
            // Convert the string 'id' to an ObjectId before querying
            var objectId = new ObjectId(id);

            // Find the document where the '_id' matches the provided ObjectId
            return await _collection.Find(Builders<T>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync();
        }

        // Inserts a new document into the collection
        public async Task CreateAsync(T entity)
        {
            // Add the provided entity to the collection
            await _collection.InsertOneAsync(entity);
        }

        public async Task<T> CreateAndReturnEntityAsync(T entity)
        {
            // Add the provided entity to the collection
            await _collection.InsertOneAsync(entity);

            // MongoDB automatically assigns the Id to the entity after insertion
            return entity;
        }


        // Updates an existing document by its ID
        public async Task UpdateAsync(string id, T entity)
        {
            // Convert the string 'id' to an ObjectId before querying
            var objectId = new ObjectId(id);

            var updateDefinition = new List<UpdateDefinition<T>>();
            var entityType = typeof(T);
            var properties = entityType.GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(entity);
                if (value != null) // Bỏ qua các trường có giá trị null
                {
                    if (property.PropertyType == typeof(List<Role>)) // Kiểm tra đặc biệt cho Role
                    {
                        // Giữ nguyên giá trị cũ 
                        continue;
                    }
                    updateDefinition.Add(Builders<T>.Update.Set(property.Name, value));
                }
            }

            if (updateDefinition.Any())
            {
                var combinedUpdate = Builders<T>.Update.Combine(updateDefinition);
                await _collection.UpdateOneAsync(Builders<T>.Filter.Eq("_id", objectId), combinedUpdate);
            }
        }

        // Deletes a document by its ID
        public async Task DeleteAsync(string id)
        {
            // Convert the string 'id' to an ObjectId before querying
            var objectId = new ObjectId(id);

            // Remove the document where the '_id' matches the provided ID
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", objectId));
        }
    }
}
