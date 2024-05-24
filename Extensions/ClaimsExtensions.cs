using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace leave_master_backend.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                // Handle the case where user is null
                return null; // or throw an exception, log an error, etc.
            }

            var uniqueNameClaim = user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/unique_name"));

            if (uniqueNameClaim != null)
            {
                return uniqueNameClaim.Value;
            }
            else
            {
                // Handle the case where the unique name claim is not found
                return null; // or throw an exception, log an error, etc.
            }
        }


    }
}