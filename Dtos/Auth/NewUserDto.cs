using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace leave_master_backend.Dtos.Auth
{
    public class NewUserDto
    {
        public ObjectId Id { get; set; } 
        public string? Email { get; set; } 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Token { get; set; }
    }
}