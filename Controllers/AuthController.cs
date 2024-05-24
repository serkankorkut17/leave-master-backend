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

namespace leave_master_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailService;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, EmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailService = emailService;
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

                if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                {
                    return BadRequest("Email already exists");
                }

                // if (!await _roleManager.RoleExistsAsync("User"))
                // {
                //     await _roleManager.CreateAsync(new ApplicationRole { Name = "User" });

                // }

                if (!await _roleManager.RoleExistsAsync(registerDto.EmployeeRole))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = registerDto.EmployeeRole });
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
                    // var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    var roleResult = await _userManager.AddToRoleAsync(user, registerDto.EmployeeRole);

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
            if (!ModelState.IsValid)
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

                if (!result.Succeeded)
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

        [HttpPost("reset-password-request")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordRequestDto.Email);
                if (user == null)
                {
                    return BadRequest("Invalid email");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var encodedToken = HttpUtility.UrlEncode(token);
                var encodedEmail = HttpUtility.UrlEncode(user.Email);
                var resetLink = $"http://localhost:3000/reset-password/{encodedToken}/{encodedEmail}";


                // Log the values used for URL generation
                Console.WriteLine($"Generating reset link for token: {token} and email: {user.Email}");

                // var resetLink = $"http://localhost:3000/reset-password/{token}/{user.Email}";

                // var resetLink = Url.Action("ResetPassword", "Auth", new { token = token, email = user.Email }, Request.Scheme);

                // Log the generated reset link
                Console.WriteLine($"Reset Link: {resetLink}");

                var message = $"Please reset your password by clicking <a href='{resetLink}'>here</a>.";
                await _emailService.SendEmailAsync(user.Email, "Password Reset Request", message);

                return Ok("Password reset request sent successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                // print resetPasswordDto
                Console.WriteLine(resetPasswordDto.Email);
                Console.WriteLine(resetPasswordDto.Password);
                Console.WriteLine(resetPasswordDto.ConfirmPassword);
                Console.WriteLine(resetPasswordDto.Token);

                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    return BadRequest("Invalid email");
                }

                if (resetPasswordDto.Password != resetPasswordDto.ConfirmPassword)
                {
                    return BadRequest("Passwords do not match");
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

                if (result.Succeeded)
                {
                    return Ok("Password reset successfully");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}