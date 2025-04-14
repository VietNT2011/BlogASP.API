using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlogASP.API.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CommentId { get; set; }

        [BsonElement("CommentText")]
        public string? CommentText { get; set; }
        
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("PostId")]
        public string? PostId {  get; set; }

        [BsonElement("Auhtor")]
        public User? Author { get; set; }
    }
}
