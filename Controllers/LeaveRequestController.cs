using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_master_backend.Dtos.Auth;
using leave_master_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using leave_master_backend.Interfaces;
using System.Web;
using leave_master_backend.Context;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;
using leave_master_backend.Extensions;
using leave_master_backend.Dtos.LeaveRequests;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_master_backend.Dtos.LeaveRequests;
using leave_master_backend.Models;
using leave_master_backend.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;




namespace leave_master_backend.Controllers
{
    [Route("api/leave-request")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly MongoDBContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public LeaveRequestController(MongoDBContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //** GET ALL LEAVE REQUESTS **//
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetLeaveRequests()
        {
            // var user = await _userManager.GetUserAsync(User);
            // var roles = await _userManager.GetRolesAsync(user);

            // if (!roles.Contains("Admin"))
            // {
            //     return Forbid();
            // }

            var leaveRequests = await _dbContext.LeaveRequests.ToListAsync();

            var leaveRequestsDto = leaveRequests.Select(lr => new GetLeaveRequestDto
            {
                Id = lr.Id.ToString(),
                UserName = lr.UserName,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                LeaveDays = lr.LeaveDays,
                Reason = lr.Reason,
            }).ToList();

            return Ok(leaveRequestsDto);
        }

        //** GET EMPLOYEE'S LEAVE REQUEST **//
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyLeaveRequest()
        {
            // Ensure user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in claims");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var leaveRequest = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.UserName == user.UserName);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return Ok(leaveRequest);
        }

        // get max leave days from user
        [HttpGet("max-leave-days")]
        [Authorize]
        public async Task<IActionResult> GetMaxLeaveDays()
        {
            // Ensure user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in claims");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var timeSpan = DateTime.Now - user.StartDate;
            var yearsOfWork = timeSpan.Days / 365;
            var maxLeaveDays = 0;

            if (yearsOfWork >= 1 && yearsOfWork < 5)
            {
                maxLeaveDays = 14;
            }
            else if (yearsOfWork >= 5 && yearsOfWork < 15)
            {
                maxLeaveDays = 20;
            }
            else if (yearsOfWork >= 15)
            {
                maxLeaveDays = 26;
            }

            int usedLeaveDays = user.UsedLeaveDaysPerYear.FirstOrDefault(kvp => kvp.Key == DateTime.Now.Year).Value;

            maxLeaveDays -= usedLeaveDays;

            return Ok(maxLeaveDays);
        }

        //** CREATE LEAVE REQUEST FOR EMPLOYEE **//
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestDto createLeaveRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in claims");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // check if user has already a leave request
            var leaveRequestInDb = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.UserName == user.UserName);

            if (leaveRequestInDb != null)
            {
                return BadRequest("User already has a leave request");
            }

            // Ensure StartDate and EndDate are not null before proceeding
            if (!createLeaveRequestDto.StartDate.HasValue || !createLeaveRequestDto.EndDate.HasValue)
            {
                return BadRequest("StartDate and EndDate are required.");
            }

            // Get the value of the nullable DateTime
            DateTime startDate = createLeaveRequestDto.StartDate.Value;
            DateTime endDate = createLeaveRequestDto.EndDate.Value;
            // add 3 hours for each date
            startDate = startDate.AddHours(3);
            endDate = endDate.AddHours(3);

            // Initialize the leave days count
            int leaveDaysCount = 0;

            // Loop through each date from start to end date
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Check if the current date is not a Saturday or Sunday
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    // Increment the leave days count
                    leaveDaysCount++;
                }
            }

            // calculate time between start date and now and get year
            var timeSpan = DateTime.Now - user.StartDate;
            var yearsOfWork = timeSpan.Days / 365;
            var maxLeaveDays = 0;

            if (yearsOfWork >= 1 && yearsOfWork < 5)
            {
                maxLeaveDays = 14;
            }
            else if (yearsOfWork >= 5 && yearsOfWork < 15)
            {
                maxLeaveDays = 20;
            }
            else if (yearsOfWork >= 15)
            {
                maxLeaveDays = 26;
            }

            int usedLeaveDays = user.UsedLeaveDaysPerYear.FirstOrDefault(kvp => kvp.Key == DateTime.Now.Year).Value;

            if (leaveDaysCount > maxLeaveDays - usedLeaveDays)
            {
                return BadRequest($"You can't take more than {maxLeaveDays - usedLeaveDays} days of leave.");
            }


            var leaveRequest = new LeaveRequest
            {
                Id = ObjectId.GenerateNewId(),
                UserName = user.UserName,
                StartDate = startDate,
                EndDate = endDate,
                LeaveDays = leaveDaysCount,
                Status = "Pending",
                Reason = createLeaveRequestDto.Reason,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Save to database using Entity Framework
            await _dbContext.LeaveRequests.AddAsync(leaveRequest);
            await _dbContext.SaveChangesAsync();

            return Ok(leaveRequest);
        }

        //** DELETE LEAVE REQUEST FOR EMPLOYEE **//
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteLeaveRequest()
        {
            // Ensure user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in claims");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var leaveRequest = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.UserName == user.UserName);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            _dbContext.LeaveRequests.Remove(leaveRequest);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        [Authorize]
        public async Task<IActionResult> GetLeaveRequestById(string id)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in claims");
            }

            // Check if the user is an admin
            var admin = await _userManager.FindByNameAsync(username);
            if (admin == null)
            {
                return BadRequest("Admin not found");
            }

            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return Unauthorized("User is not authorized to approve leave requests");
            }

            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(new ObjectId(id));
            if (leaveRequest == null)
            {
                return NotFound("Leave request not found");
            }

            var user = await _userManager.FindByNameAsync(leaveRequest.UserName);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles == null || !userRoles.Any())
            {
                return BadRequest("User role not found");
            }

            var userRole = userRoles[0];

            var allLeaves = await _dbContext.Leaves.ToListAsync();

            int sameRoleCollisions = 0;
            int differentRoleCollisions = 0;

            foreach (var leave in allLeaves)
            {
                if ((leave.StartDate <= leaveRequest.EndDate) && (leave.EndDate >= leaveRequest.StartDate))
                {
                    var leaveUser = await _userManager.FindByNameAsync(leave.UserName);
                    if (leaveUser == null) continue;

                    var leaveUserRoles = await _userManager.GetRolesAsync(leaveUser);
                    if (leaveUserRoles.Contains(userRole))
                    {
                        sameRoleCollisions++;
                    }
                    else
                    {
                        differentRoleCollisions++;
                    }
                }
            }

            string recommendation;
            if (sameRoleCollisions > 3)
            {
                recommendation = "High overlap: Strongly recommend refusing the leave request due to multiple overlapping leaves in the same role.";
            }
            else if (sameRoleCollisions > 0)
            {
                recommendation = "Moderate overlap: Consider refusing the leave request due to some overlapping leaves in the same role.";
            }
            else if (differentRoleCollisions > 5)
            {
                recommendation = "High overlap in different roles: Recommend discussing with the employee due to significant overlapping leaves in different roles.";
            }
            else if (differentRoleCollisions > 2)
            {
                recommendation = "Moderate overlap in different roles: Leave request can be approved, but be aware of some overlapping leaves in different roles.";
            }
            else
            {
                recommendation = "No significant overlap: Leave request can be approved with no significant overlapping leaves found.";
            }

            return Ok(new
            {
                leaveRequest,
                SameRoleCollisions = sameRoleCollisions,
                DifferentRoleCollisions = differentRoleCollisions,
                Recommendation = recommendation
            });
        }

    }
}