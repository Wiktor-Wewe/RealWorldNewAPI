using Microsoft.AspNetCore.Mvc;
using RealWorldNew.Common;
using RealWorldNew.Common.Models;

namespace RealWorldNewAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentService _commnetService;
        public CommentController(ICommentService commnetService)
        {
            _commnetService = commnetService;
        }

        [HttpPost("articles/{title}-{id}/comments")]
        public async Task<IActionResult> AddCommnet([FromBody]CommentPack pack, [FromRoute]string title, [FromRoute]int id)
        {
            var result = await _commnetService.AddCommnetAsync(pack, User.Identity.Name, title, id);
            return Ok(result);
        }

        [HttpGet("articles/{title}-{id}/comments")]
        public async Task<IActionResult> GetCommnents([FromRoute]string title, [FromRoute]int id)
        {
            var result = await _commnetService.GetCommentsAsync(title, id);
            return Ok(result);
        }

        [HttpDelete("articles/{title}-{id}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute]string title, [FromRoute]int id, [FromRoute]int commentId)
        {
            await _commnetService.DeleteCommentAsync(title, id, commentId);
            return Ok();
        }
    }
}
