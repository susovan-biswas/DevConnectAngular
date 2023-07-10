using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(ITokenService tokenService,
                                 UserManager<AppUser> userManager,
                                 IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            

        }

        [HttpPost("register")] //POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("User name already exists");
            
            var user = _mapper.Map<AppUser>(registerDto); 
            user.UserName = registerDto.UserName.ToLower();

            var result = await _userManager.CreateAsync(user,registerDto.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user,"User");
            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);
           
            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                FullName = user.FullName

            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x =>
            x.UserName == loginDto.UserName.ToLower());

            if (user == null) return Unauthorized("invalid user");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if(!result) return Unauthorized("Invalid Password");
            
            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsProfile)?.Url,
                FullName = user.FullName
            };



        }



        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}