using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common.Exceptions;
using RealWorldNew.DAL.Entities;
using RealWorldNew.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNewAPI.Test.ArticleServiceTests
{
    [Category("DeleteArticleAsync")]
    public class DeleteArticleAsyncTests
    {
        [Test]
        public async Task DeleteArticleAsync_CantFindArticle_ThrowArticleException()
        {
            var title = "some title";
            var id = 21;

            var articleRepositories = new Mock<IArticleRepositories>();
            articleRepositories.Setup(x => x.FindBySlugAsync(title, id)).ReturnsAsync((Article)null);

            var logger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(null, articleRepositories.Object, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<ArticleException>().And.Message.EqualTo("Can't find article while deleting"), async delegate { await articleService.DeleteArticleAsync(title, id); });
        }
    }
}
