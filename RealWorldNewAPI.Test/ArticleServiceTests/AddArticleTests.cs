using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common;
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
    [Category("AddArticle")]
    public class AddArticleTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task AddArticle_WithCorrectData_ReturnArticleUploadResponse()
        {
            //Arrange
            var userId = "someuserid";
            var pack = new ArticleUpload()
            {
                body = "text",
                description = "desc",
                tagList = new List<string>()
                {
                    "some", "tags"
                },
                title = "some title"
            };

            var article = new Article()
            {
                Id = 12,
                Title = "some title"
            };

            var tags = new List<Tag>()
            {
                new Tag()
                {
                    Name = "some"
                },
                new Tag()
                {
                    Name = "tags"
                }
            };

            var tag = new Tag()
            {
                Name = "dummy"
            };

            var user = new User()
            {
                UserName = "username",
                Email = "email@wp.pl",
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            var articleRepositories = new Mock<IArticleRepositories>();
            articleRepositories.Setup(x => x.GetAllTagsAsync()).ReturnsAsync(tags);
            articleRepositories.Setup(x => x.GetTagByNameAsync(It.IsAny<string>())).ReturnsAsync(tag);

            var articleService = new ArticleService(userManager.Object, articleRepositories.Object, null);

            //Act
            var result = await articleService.AddArticle(userId, pack);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.article.body, Is.EqualTo(pack.body));
            Assert.That(result.article.title, Is.EqualTo(pack.title));
        }

        [Test]
        public async Task AddArticle_CantFindUser_ThrowArticleException()
        {
            //Arrange
            var userId = "someuserid";
            var pack = new ArticleUpload()
            {
                body = "text",
                description = "desc",
                tagList = new List<string>()
                {
                    "some", "tags"
                },
                title = "some title"
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User)null);

            var logger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(userManager.Object, null, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<ArticleException>().And.Message.EqualTo("Can't find user while adding article"), async delegate { await articleService.AddArticle(userId, pack); });
        }
    }
}
