using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealWorldNew.DAL.Entities;

namespace RealWorldNewAPI.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class RealWorldController : ControllerBase
    {
        private IRealWorldService _realWorldService;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public RealWorldController(IRealWorldService realWorldService, UserManager<User> userManager, SignInManager<User> signInMenager)
        {
            _realWorldService = realWorldService;
            _userManager = userManager;
            _signInManager = signInMenager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var result = _realWorldService.GetAllUsers();
            return Ok(result);
        }

        [HttpPost("register")]
        public ActionResult AddUser([FromBody]User user)
        {
            var userName = _realWorldService.AddUser(user);
            return Created($"/profile/{userName}", null);
        }

        [HttpPost("test")]
        public async Task<IActionResult> Register()
        {
            User user = await _userManager.FindByNameAsync("TestUser");
            if (user == null)
            {
                user = new User();
                user.UserName = "TestUser";
                user.Email = "TestUser@test.com";
                user.ShortBio = "bla bla bla";
                user.UrlProfile = "www.pic.com/dsf3232rdf.jpg";

                IdentityResult result = await _userManager.CreateAsync(user, "Test123!");
            }
            return Ok(user);
        }
    }
}
