using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_master_backend.Dtos
{
    public class GetEmployeeInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string SignupCode { get; set; } = string.Empty;
    }
}