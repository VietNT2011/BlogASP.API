﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlogASP.API.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? PostId { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; } = null!;

        [BsonElement("Content")]
        public string Content { get; set; } = null!;

        [BsonElement("AuthorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; } = null!;// one to many User

        [BsonElement("CategoryIds")]
        public List<string> CategoryIds { get; set; } = new(); // many to many Category

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
