using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using RealWorldNew.BAL;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealWorldNewAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IPackingService _packingService; 

        public UserController(IUserService userService, UserManager<User> userManager, IPackingService packingService)
        {
            _userService = userService;
            _userManager = userManager;
            _packingService = packingService;
        }

        [HttpPost("users")]
        public async Task<IActionResult> Register([FromBody]RegisterUserPack userPack)
        {
            try
            {
                var user = await _userService.Register(userPack);
                return Ok(_packingService.PackUser(user, _userService.GetToken(user)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpPost("users/login")]
        public async Task<IActionResult> Login([FromBody]LoginUserPack modelPack)
        {
            try
            {
                var user = await _userService.Login(modelPack);
                return Ok(_packingService.PackUser(user, _userService.GetToken(user)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetMyInfo()
        {
            var user = await _userService.GetMyInfo(User.Identity.Name);
            if(user == null) return NotFound();

            string token = this.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            user.user.token = token;
            return Ok(user);
        }

        [HttpGet("profiles/{userName}")]
        public async Task<IActionResult> LoadProfile([FromRoute]string username)
        {
            try
            {
                var result = await _userService.LoadProfile(username, User.Identity.Name);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("user")]
        public async Task<IActionResult> UpdatePrfile([FromBody]ChangeProfileContainer newProfileSettings)
        {
            try
            {
                await _userService.UpdateUser(User.Identity.Name, newProfileSettings.user);
                return Ok(newProfileSettings);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
