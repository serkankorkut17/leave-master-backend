using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_master_backend.Context;
using leave_master_backend.Models;
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
        public LeaveRequestController(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaveRequests()
        {
            var leaveRequests = await _dbContext.LeaveRequests.ToListAsync();
            return Ok(leaveRequests);
        }

        [HttpGet("{id:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        public async Task<IActionResult> GetLeaveRequest(string id)
        {
            ObjectId objectId = new ObjectId(id);
            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(objectId);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            return Ok(leaveRequest);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] LeaveRequest leaveRequest)
        {
            await _dbContext.LeaveRequests.AddAsync(leaveRequest);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id.ToString() }, leaveRequest);
        }

        [HttpPut("{id:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        public async Task<IActionResult> UpdateLeaveRequest([FromRoute] string id, [FromBody] LeaveRequest leaveRequest)
        {
            ObjectId objectId = new ObjectId(id);
            var leaveRequestInDb = await _dbContext.LeaveRequests.FindAsync(objectId);

            if (leaveRequestInDb == null)
            {
                return NotFound();
            }

            leaveRequestInDb.UserId = leaveRequest.UserId;
            leaveRequestInDb.StartDate = leaveRequest.StartDate;
            leaveRequestInDb.EndDate = leaveRequest.EndDate;
            leaveRequestInDb.LeaveDays = leaveRequest.LeaveDays;
            leaveRequestInDb.Status = leaveRequest.Status;
            leaveRequestInDb.Reason = leaveRequest.Reason;
            leaveRequestInDb.CreatedAt = leaveRequest.CreatedAt;
            leaveRequestInDb.UpdatedAt = leaveRequest.UpdatedAt;
            leaveRequestInDb.ApproverId = leaveRequest.ApproverId;
            leaveRequestInDb.ApproverComment = leaveRequest.ApproverComment;

            await _dbContext.SaveChangesAsync();
            return Ok(leaveRequestInDb);
        }

        [HttpDelete("{id:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        public async Task<IActionResult> DeleteLeaveRequest(string id)
        {
            ObjectId objectId = new ObjectId(id);
            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(objectId);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            _dbContext.LeaveRequests.Remove(leaveRequest);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}