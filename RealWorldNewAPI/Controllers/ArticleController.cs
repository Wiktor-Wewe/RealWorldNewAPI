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
        public async IActionResult AddArticle([FromBody] ArticleUpload pack)
        {
            var result = _articleService.AddArticle(User.Identity.Name, pack);
            return Ok(result);
        }
    }
}
