using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_master_backend.Dtos.LeaveRequests
{
    public class GetLeaveRequestDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveDays { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}