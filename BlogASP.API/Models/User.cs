using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlogASP.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        [BsonElement("Username")]
        public string? UserName { get; set; }

        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonElement("PasswordHash")]
        public string? PasswordHash { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("Posts")]
        public List<string> Posts { get; set; } = new(); // List Post IDs
    }
}
