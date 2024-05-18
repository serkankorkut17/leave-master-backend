using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_master_backend.Dtos.User;

namespace leave_master_backend.Mappers
{
    public static class UserMappers
    {
        public static UserDto ToUserDto(this Models.User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                // Password = user.Password,
                // Role = user.Role,
                // StartDate = user.StartDate,
                // UsedLeaveDaysPerYear = user.UsedLeaveDaysPerYear
            };
        }
    }
}