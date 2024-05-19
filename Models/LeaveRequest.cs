using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using MongoDbGenericRepository.Attributes;

namespace leave_master_backend.Models
{
    [CollectionName("LeaveRequests")]
    public class LeaveRequest
    {
        public ObjectId Id { get; set; }

        // public ObjectId UserId { get; set; }
        // public required ApplicationUser User { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int LeaveDays { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // public ObjectId? ApproverId { get; set; }
        // public ApplicationUser? Approver { get; set; }

        // public string? ApproverComment { get; set; } = string.Empty;

    }
}