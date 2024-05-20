using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_master_backend.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);

    }
}