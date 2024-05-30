using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using leave_master_backend.Context;
using leave_master_backend.Dtos.LeaveRequests;
using leave_master_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Microsoft.EntityFrameworkCore;

namespace leave_master_backend.Controllers
{
    [Route("api/leaves")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly MongoDBContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaveController(MongoDBContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Approve a Leave Request and Create Leave
        [HttpPost("approve/{requestId:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        [Authorize]
        public async Task<IActionResult> ApproveLeaveRequest(string requestId)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in claims");
            }

            // Check if the user is an admin
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Unauthorized("User is not authorized to approve leave requests");
            }

            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(new ObjectId(requestId));

            if (leaveRequest == null)
            {
                return NotFound("Leave request not found");
            }

            var employee = await _userManager.FindByNameAsync(leaveRequest.UserName);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            // Calculate the year from the start date
            var startDate = leaveRequest.StartDate;
            int year = startDate.Year;

            // Update the UsedLeaveDaysPerYear for the employee
            var usedLeaveDaysPerYear = employee.UsedLeaveDaysPerYear;
            var leaveDays = leaveRequest.LeaveDays;

            // Check if the year already exists in the list
            var existingEntry = usedLeaveDaysPerYear.FirstOrDefault(entry => entry.Key == year);
            if (existingEntry.Equals(default(KeyValuePair<int, int>)))
            {
                // Year not found, add new entry
                usedLeaveDaysPerYear.Add(new KeyValuePair<int, int>(year, leaveDays));
            }
            else
            {
                // Year found, update existing entry
                var updatedEntry = new KeyValuePair<int, int>(year, existingEntry.Value + leaveDays);
                var index = usedLeaveDaysPerYear.IndexOf(existingEntry);
                usedLeaveDaysPerYear[index] = updatedEntry;
            }

            // Update the employee's used leave days in the database
            await _userManager.UpdateAsync(employee);

            // Create the new leave
            var leave = new Leave
            {
                Id = ObjectId.GenerateNewId(),
                UserName = leaveRequest.UserName,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                LeaveDays = leaveRequest.LeaveDays,
                Reason = leaveRequest.Reason,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ApproverName = username
            };

            await _dbContext.Leaves.AddAsync(leave);
            _dbContext.LeaveRequests.Remove(leaveRequest);
            await _dbContext.SaveChangesAsync();

            return Ok(leave);
        }

        // Refuse a Leave Request and Delete the Request
        [HttpPost("refuse/{requestId:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        [Authorize]
        public async Task<IActionResult> RefuseLeaveRequest([FromRoute] string requestId)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in claims");
            }

            // check user role is admin
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Unauthorized("User is not authorized to approve leave requests");
            }

            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(new ObjectId(requestId));

            if (leaveRequest == null)
            {
                return NotFound("Leave request not found");
            }

            _dbContext.LeaveRequests.Remove(leaveRequest);
            await _dbContext.SaveChangesAsync();

            return Ok("Leave request refused and deleted");
        }

        // Get Leaves by Month and Year
        [HttpPost("leaves-by-month-year")]
        [Authorize]
        public async Task<IActionResult> GetLeavesByMonthYear([FromBody] LeaveRequestByMonthYearDto requestDto)
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in claims");
            }

            // Get the user role
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles[0];

            // Query leaves for the specified month and year
            var leaves = await _dbContext.Leaves
                .Where(l => (l.StartDate.Year == requestDto.Year && l.StartDate.Month == requestDto.Month)
                            || (l.EndDate.Year == requestDto.Year && l.EndDate.Month == requestDto.Month))
                .ToListAsync();

            var sameRoleDays = new HashSet<int>();
            var otherRoleDays = new HashSet<int>();

            foreach (var leave in leaves)
            {
                var leaveUser = await _userManager.FindByNameAsync(leave.UserName);
                if (leaveUser == null) continue;

                var leaveUserRoles = await _userManager.GetRolesAsync(leaveUser);
                bool isSameRole = leaveUserRoles.Contains(role);

                DateTime current = leave.StartDate;
                while (current <= leave.EndDate)
                {
                    if (current.Year == requestDto.Year && current.Month == requestDto.Month)
                    {
                        if (isSameRole)
                        {
                            sameRoleDays.Add(current.Day);
                        }
                        else
                        {
                            otherRoleDays.Add(current.Day);
                        }
                    }
                    current = current.AddDays(1);
                }
            }

            return Ok(new
            {
                SameRoleDays = sameRoleDays.OrderBy(d => d).ToArray(),
                OtherRoleDays = otherRoleDays.OrderBy(d => d).ToArray()
            });
        }

    }

    public class LeaveRequestByMonthYearDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
