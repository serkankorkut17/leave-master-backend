using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using leave_master_backend.Context;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using leave_master_backend.Dtos.User;
using leave_master_backend.Mappers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace leave_master_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MongoDBContext _dbContext;
        public AuthController(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            
            var usersDto = users.Select(user => user.ToUserDto());
            return Ok(usersDto);
        }

        [HttpGet("{id:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        public async Task<IActionResult> GetUser(string id)
        {
            ObjectId objectId = new ObjectId(id);
            var user = await _dbContext.Users.FindAsync(objectId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.ToUserDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] Models.User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id.ToString() }, user);
        }

        [HttpPut("{id:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserDto user)
        {
            ObjectId objectId = new ObjectId(id);
            var userInDb = await _dbContext.Users.FindAsync(objectId);

            if (userInDb == null)
            {
                return NotFound();
            }
            // change only if the value is not null or empty string!!!!!!!!!!!!!!!!!!!!!!

            userInDb.FirstName = user.FirstName ?? userInDb.FirstName;
            userInDb.LastName = user.LastName ?? userInDb.LastName;
            userInDb.Email = user.Email ?? userInDb.Email;
            userInDb.Password = user.Password ?? userInDb.Password;
            userInDb.Role = user.Role ?? userInDb.Role;

            // userInDb.StartDate = user.StartDate; 
            userInDb.UsedLeaveDaysPerYear = user.UsedLeaveDaysPerYear ?? userInDb.UsedLeaveDaysPerYear;

            _dbContext.Users.Update(userInDb);
            await _dbContext.SaveChangesAsync();
            return Ok(userInDb.Id.ToString());
        }

        [HttpDelete("{id:regex(^[[0-9a-fA-F]]{{24}}$)}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            ObjectId objectId = new ObjectId(id);
            var user = await _dbContext.Users.FindAsync(objectId);
            if (user == null)
            {
                return NotFound();
            }
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}