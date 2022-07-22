using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealWorldNew.Common;
using RealWorldNew.DAL.Entities;

namespace RealWorldNewAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;
        public FollowController(IFollowService followService, UserManager<User> userManager)
        {
            _followService = followService;
        }

        [HttpPost("profiles/{username}/follow")]
        public async Task<IActionResult> FollowUser([FromRoute] string username)
        {
            var response = await _followService.FollowUser(User.Identity.Name, username);
            return Ok(response);
        }

        [HttpDelete("profiles/{username}/follow")]
        public async Task<IActionResult> UnfollowUser([FromRoute] string username)
        {
            var response = await _followService.UnfollowUser(User.Identity.Name, username);
            return Ok(response);
        }

        [HttpPost("articles/{title}-{id}/favorite")]
        public async Task<IActionResult> AddArticleToFavorite([FromRoute] string title, [FromRoute] int id)
        {
            var response = await _followService.AddArticleToFavorite(title, id, User.Identity.Name);
            return Ok(response);
        }

        [HttpDelete("articles/{title}-{id}/favorite")]
        public async Task<IActionResult> RemoveArticleFromFavorite([FromRoute] string title, [FromRoute] int id)
        {
            var response = await _followService.RemoveArticleFromFavorite(title, id, User.Identity.Name);
            return Ok(response);
        }
    }
}
