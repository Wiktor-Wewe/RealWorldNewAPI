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
    [Category("GetArticle")]
    public class GetArticleTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task GetArticle_WithCorrectData_ReturnArticleUploadResponse()
        {
            //Arrange
            var currentUserId = "some id";
            var title = "some title";
            var id = 12;

            var article = new Article()
            {
                Title = title,
                Text = "some text"
            };

            var user = new User()
            {
                UserName = "some username",
                Email = "someemail@wp.pl"
            };

            var articleRepositories = new Mock<IArticleRepositories>();
            articleRepositories.Setup(x => x.FindBySlugAsync(title, id)).ReturnsAsync(article);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(currentUserId)).ReturnsAsync(user);

            var articleService = new ArticleService(userManager.Object, articleRepositories.Object, null);

            //Act
            var result = await articleService.GetArticle(currentUserId, title, id);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.article.body, Is.EqualTo(article.Text));
            Assert.That(result.article.title, Is.EqualTo(article.Title));
        }

        [Test]
        public async Task GetArticle_CantFindArticle_ThrowArticleException()
        {
            //Arrange
            var currentUserId = "some id";
            var title = "some title";
            var id = 12;

            var articleRepositories = new Mock<IArticleRepositories>();
            articleRepositories.Setup(x => x.FindBySlugAsync(title, id)).ReturnsAsync((Article)null);

            var logger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(null, articleRepositories.Object, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<ArticleException>().And.Message.EqualTo("Can't find article"), async delegate { await articleService.GetArticle(currentUserId, title, id); });
        }
    }
}
