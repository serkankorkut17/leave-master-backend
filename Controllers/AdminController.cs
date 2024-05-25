using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_master_backend.Context;
using leave_master_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace leave_master_backend.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly MongoDBContext _dbContext;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, MongoDBContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        // [Authorize
        // (Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await Task.Run(() => _userManager.Users.ToList());
            return Ok(users);
        }

        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // get role of the user
        [HttpGet("role")]
        [Authorize]
        public async Task<IActionResult> GetUserRole()
        {
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

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }


        // [HttpPut("{username}")]
        // [Authorize]
        // public async Task<IActionResult> UpdateUser([FromRoute] string username, [FromBody] ApplicationUser user)
        // {
        //     var userToUpdate = await _userManager.FindByNameAsync(username);
        //     if (userToUpdate == null)
        //     {
        //         return NotFound();
        //     }

        //     userToUpdate.FirstName = user.FirstName;
        //     userToUpdate.LastName = user.LastName;
        //     userToUpdate.Email = user.Email;

        //     var result = await _userManager.UpdateAsync(userToUpdate);
        //     if (result.Succeeded)
        //     {
        //         return Ok(userToUpdate);
        //     }

        //     return BadRequest(result.Errors);
        // }



    }
}