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
    //[ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
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
            var user = await _userService.Register(userPack);
            return Ok(_packingService.PackUser(user, _userService.GetToken(user)));
        }

        [HttpPost("users/login")]
        public async Task<IActionResult> Login([FromBody]LoginUserPack modelPack)
        {
            var user = await _userService.Login(modelPack);
            return Ok(_packingService.PackUser(user, _userService.GetToken(user)));
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
        public async Task<IActionResult> LoadProfile([FromRoute]string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound();
            
            var isFollowed = await _userService.IsFollowedUser(User.Identity.Name, userName);
            ProfileView packProfile = _packingService.PackUserToProfileView(user, isFollowed);

            return Ok(_packingService.PackProfileView(packProfile));
        }

        [HttpPut("user")]
        public async Task<IActionResult> UpdatePrfile([FromBody]ChangeProfileContainer newProfileSettings)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.Name);
            if (user == null) return NotFound();
            await _userService.ChangeUser(user, newProfileSettings.user);
            return Ok(newProfileSettings);
        }

        [HttpPost("profiles/{username}/follow")]
        public async Task<IActionResult> FollowUser([FromRoute]string username)
        {
            var userToFollow = await _userManager.FindByNameAsync(username);
            var user = await _userManager.FindByIdAsync(User.Identity.Name);
            var userContainer = _userService.FollowUser(user, userToFollow);
            return Ok(userContainer);
        }
    }
}
