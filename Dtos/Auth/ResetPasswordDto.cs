using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_master_backend.Dtos.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}