using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace leave_master_backend.Dtos.User
{
    public class UserDto
    {
        public ObjectId? Id { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public List<KeyValuePair<int, int>>? UsedLeaveDaysPerYear { get; set; } = new List<KeyValuePair<int, int>>();

    }
}