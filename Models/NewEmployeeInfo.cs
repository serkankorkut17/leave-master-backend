using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace leave_master_backend.Models
{
    public class NewEmployeeInfo
    {
        public DateTime StartDate { get; set; }
        public string SignupCode { get; set; } = string.Empty;
    }
}