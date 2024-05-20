using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_master_backend.Dtos.Auth
{
    public class ResetPasswordRequestDto
    {
        public string Email { get; set; }
    }
}