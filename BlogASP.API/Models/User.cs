using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlogASP.API.Models
{
    [BsonIgnoreExtraElements]
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

        [BsonElement("DOB")]
        public DateOnly DOB { get; set; }

        [BsonElement("AvatarURL")]
        public string? AvatarURL { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("PasswordResetToken")]
        public string? PasswordResetToken { get; set; }

        [BsonElement("PasswordResetTokenExpiry")]
        public DateTime? PasswordResetTokenExpiry { get; set; }

        [BsonElement("Roles")]
        public List<Role>? Role { get; set; } = new List<Role>();
    }
}
