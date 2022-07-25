using Microsoft.AspNetCore.Mvc;
using RealWorldNew.Common;
using RealWorldNew.Common.Models;

namespace RealWorldNewAPI.Controllers
{
    [Route("api")]
    //[ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpPost("articles")]
        public async Task<IActionResult> AddArticle([FromBody] ArticleUploadPack pack)
        {
            var result = await _articleService.AddArticle(User.Identity.Name, pack.article);
            return Ok(result);
        }

        [HttpGet("articles/{title}-{id}")]
        public async Task<IActionResult> GetArticle([FromRoute] string title, [FromRoute] int id)
        {
            var result = await _articleService.GetArticle(User.Identity.Name, title, id);
            return Ok(result);
        }

        [HttpGet("articles")]
        public async Task<IActionResult> GetArticles([FromQuery] string favorited, [FromQuery] string author, [FromQuery] int limit, [FromQuery] int offset)
        {
            var result = await _articleService.GetArticles(favorited, author, limit, offset, User.Identity.Name);
            return Ok(result);
        }

        [HttpGet("articles/feed")]
        public async Task<IActionResult> GetArticlesFeed([FromQuery] int limit, [FromQuery] int offset)
        {
            var result = await _articleService.GetArticlesFeed(limit, offset, User.Identity.Name);
            return Ok(result);
        }

        [HttpDelete("articles/{title}-{id}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] string title, [FromRoute] int id)
        {
            await _articleService.DeleteArticleAsync(title, id);
            return Ok();
        }

        [HttpPut("articles/{title}-{id}")]
        public async Task<IActionResult> EditArticle([FromBody] ArticleUploadResponse pack, [FromRoute] string title, [FromRoute] int id)
        {
            var result = await _articleService.EditArticleAsync(pack, title, id);
            return Ok(result);
        }

        [HttpGet("tags")]
        public async Task<IActionResult> GetPopularTags()
        {
            var result = await _articleService.GetPopularTags();
            return Ok(result);
        }
    }
}
