using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using api.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManger;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signinManger;

        public AccountController(UserManager<AppUser> userManger, ITokenService tokenService, SignInManager<AppUser> signinManger)
        {
            _userManger = userManger;
            _tokenService = tokenService;
            _signinManger = signinManger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManger.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
            if (user == null)
                return Unauthorized("Invalid Username or Password");
            var result = await _signinManger.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid Username or Password");
            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                };
                var createdUser = await _userManger.CreateAsync(appUser, registerDto.Password!);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManger.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new NewUserDto
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                            Token = _tokenService.CreateToken(appUser)
                        });
                    }
                    else  return StatusCode(500, roleResult.Errors);
                }
                else  return StatusCode(500, createdUser.Errors);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

    }
}