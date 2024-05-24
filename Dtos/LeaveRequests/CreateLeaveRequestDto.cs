using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_master_backend.Dtos.LeaveRequests
{
    public class CreateLeaveRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Reason { get; set; } = string.Empty;
    }
}