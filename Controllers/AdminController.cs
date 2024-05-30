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
using leave_master_backend.Dtos.Auth;
using System.Text;
using leave_master_backend.Interfaces;
using System.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson;
using leave_master_backend.Dtos;

namespace leave_master_backend.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly MongoDBContext _dbContext;
        private readonly ITokenService _tokenService;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, MongoDBContext dbContext, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        [HttpGet("get-users")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
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

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var users = await _userManager.Users.ToListAsync();
                return Ok(users);
            }
            else
            {
                return Unauthorized();
            }
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

            return Ok(roles[0]);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (registerDto.Password is null)
            {
                return BadRequest("Password cannot be null");
            }

            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }

            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                return BadRequest("Email already exists");
            }

            // check admin role is exist
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            }

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName ?? "",
                LastName = registerDto.LastName ?? "",
                StartDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");

                if (roleResult.Succeeded)
                {
                    return Ok(new NewUserDto
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Token = _tokenService.CreateToken(user)
                    });
                }
            }

            return BadRequest(result.Errors);
        }

        // create new employee info
        [HttpPost("new-employee-info")]
        [Authorize]
        public async Task<IActionResult> CreateNewEmployeeInfo([FromBody] NewEmployeeInfo newEmployeeInfo)
        {
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

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Unauthorized();
            }

            var newEmployee = new NewEmployeeInfo
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = newEmployeeInfo.StartDate,
                SignupCode = newEmployeeInfo.SignupCode
            };

            await _dbContext.NewEmployeeInfos.AddAsync(newEmployee);
            await _dbContext.SaveChangesAsync();

            return Ok(newEmployee);
        }

        // get new employee info
        [HttpGet("new-employee-infos")]
        [Authorize]
        public async Task<IActionResult> GetNewEmployeeInfo()
        {
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

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Unauthorized();
            }

            var newEmployeeInfos = await _dbContext.NewEmployeeInfos.ToListAsync();

            // convert ObjectId to string
            var newEmployeeInfosDto = newEmployeeInfos.Select(newEmployeeInfo => new GetEmployeeInfoDto
            {
                Id = newEmployeeInfo.Id.ToString(),
                StartDate = newEmployeeInfo.StartDate,
                SignupCode = newEmployeeInfo.SignupCode
            });

            return Ok(newEmployeeInfosDto);
        }

        // delte new employee info
        [HttpDelete("new-employee-info/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNewEmployeeInfo(string id)
        {
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

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Unauthorized();
            }

            var newEmployeeInfo = await _dbContext.NewEmployeeInfos.FindAsync(ObjectId.Parse(id));
            if (newEmployeeInfo == null)
            {
                return NotFound();
            }

            _dbContext.NewEmployeeInfos.Remove(newEmployeeInfo);
            await _dbContext.SaveChangesAsync();

            return Ok(newEmployeeInfo);
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