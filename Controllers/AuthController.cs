using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_master_backend.Dtos.Auth;
using leave_master_backend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using leave_master_backend.Interfaces;

namespace leave_master_backend.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (registerDto.Password != registerDto.ConfirmPassword)
                {
                    return BadRequest("Passwords do not match");
                }

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = "User" });

                }

                var user = new ApplicationUser
                {
                    Email = registerDto.Email,
                    UserName = registerDto.Email,
                    FirstName = registerDto.FirstName ?? "",
                    LastName = registerDto.LastName ?? "",
                    StartDate = registerDto.StartDate.HasValue ? (DateTime)registerDto.StartDate : DateTime.MinValue
                };

                if (registerDto.Password is null)
                {
                    return BadRequest("Password cannot be null");
                }

                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    
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
                    else
                    {
                        return BadRequest(roleResult.Errors);
                    }
                }
                else
                {
                    return BadRequest(createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user is null)
                {
                    return Unauthorized("Invalid email or password");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if(!result.Succeeded)
                {
                    return Unauthorized("Invalid email or password");
                }

                return Ok(new NewUserDto
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = _tokenService.CreateToken(user)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}