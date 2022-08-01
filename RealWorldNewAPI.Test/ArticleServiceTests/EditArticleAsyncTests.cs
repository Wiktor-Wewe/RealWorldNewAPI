using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common.Exceptions;
using RealWorldNew.Common.Models;
using RealWorldNew.DAL.Entities;
using RealWorldNew.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNewAPI.Test.ArticleServiceTests
{
    [Category("EditArticleAsync")]
    public class EditArticleAsyncTests
    {
        [Test]
        public async Task EditArticleAsync_WithCorrectData_ReturnArticleUploadResponse()
        {
            var pack = new ArticleUploadResponse()
            {
                article = new articleAUP()
                {
                    body = "new text",
                    title = "new some title",
                    description = "new some description",
                    tagList = new List<string>()
                }
            };

            var article = new Article()
            {
                Text = "text",
                Topic = "some description",
                Title = "some title",
            };

            var tag = new Tag()
            {
                Name = "yes"
            };

            var tags = new List<Tag>()
            {
                tag
            };

            var title = "some title";
            var id = 12;

            var articleRepositories = new Mock<IArticleRepositories>();
            articleRepositories.Setup(x => x.FindBySlugAsync(title, id)).ReturnsAsync(article);
            articleRepositories.Setup(x => x.GetAllTagsAsync()).ReturnsAsync(tags);
            articleRepositories.Setup(x => x.GetTagByNameAsync(It.IsAny<string>())).ReturnsAsync(tag);

            var articleService = new ArticleService(null, articleRepositories.Object, null);

            //Act
            var result = await articleService.EditArticleAsync(pack, title, id);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.article.body, Is.EqualTo(pack.article.body));
            Assert.That(result.article.title, Is.EqualTo(pack.article.title));
            Assert.That(result.article.description, Is.EqualTo(pack.article.description));
        }

        [Test]
        public async Task EditArticleAsync_CantFindArticle_ThrowArticleException()
        {
            var pack = new ArticleUploadResponse()
            {
                article = new articleAUP()
                {
                    body = "new text",
                    title = "new some title",
                    description = "new some description",
                    tagList = new List<string>()
                }
            };

            var title = "some title";
            var id = 12;

            var articleRepositories = new Mock<IArticleRepositories>();
            articleRepositories.Setup(x => x.FindBySlugAsync(title, id)).ReturnsAsync((Article)null);

            var logger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(null, articleRepositories.Object, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<ArticleException>().And.Message.EqualTo("Can't find article while editing"), async delegate { await articleService.EditArticleAsync(pack, title, id); });
        }
    }
}
