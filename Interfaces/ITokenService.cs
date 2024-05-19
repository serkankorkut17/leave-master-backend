using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_master_backend.Models;

namespace leave_master_backend.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}