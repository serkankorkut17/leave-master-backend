using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace leave_master_backend.Models
{
    [CollectionName("Leaves")]
    public class Leave
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonIgnore]
        public ApplicationUser User { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int LeaveDays { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ApproverId { get; set; }

        [BsonIgnore]
        public ApplicationUser Approver { get; set; }
    }
}
