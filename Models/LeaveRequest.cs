using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace leave_master_backend.Models
{
    public class LeaveRequest
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; } = string.Empty;//change to ObjectId
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int LeaveDays { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? ApproverId { get; set; } = string.Empty;//change to ObjectId

        public string? ApproverComment { get; set; } = string.Empty;

    }
}