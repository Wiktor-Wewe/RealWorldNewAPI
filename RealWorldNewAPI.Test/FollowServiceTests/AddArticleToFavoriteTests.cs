using Microsoft.AspNetCore.Identity;
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

namespace RealWorldNewAPI.Test.FollowServiceTests
{
    [Category("AddArticleToFavorite")]
    public class AddArticleToFavoriteTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task AddArticleToFavorite_WithEveryDataOkay_ReturnArticleAUP()
        {
            //Arrange
            var inputTitle = "some title";
            var inputId = 12;
            var inputCurrentUserId = "some id";

            var article = new Article()
            {
                Title = "some title",
            };

            var user = new User()
            {
                Id = inputCurrentUserId,
            };

            var followRepositories = new Mock<IFollowRepositories>();
            followRepositories.Setup(x => x.GetArticleBySlugAsync(inputTitle, inputId, true)).ReturnsAsync(article);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var followService = new FollowService(userManager.Object, followRepositories.Object, null);

            //Act
            var act = await followService.AddArticleToFavorite(inputTitle, inputId, inputCurrentUserId);

            //Assert
            Assert.IsNotNull(act);
            Assert.That(act.title, Is.EqualTo(article.Title));
        }

        [Test]
        public async Task AddArticleToFavorite_CantUpdateDatabase_ReturnLikeException()
        {
            //Arrange
            var inputTitle = "some title";
            var inputId = 12;
            var inputCurrentUserId = "some id";

            var article = new Article()
            {
                Title = "some title",
            };

            var user = new User()
            {
                Id = inputCurrentUserId,
            };

            var followRepositories = new Mock<IFollowRepositories>();
            followRepositories.Setup(x => x.GetArticleBySlugAsync(inputTitle, inputId, true)).ReturnsAsync(article);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed());

            var logger = new Mock<ILogger<FollowService>>();

            var followService = new FollowService(userManager.Object, followRepositories.Object, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<LikeException>().And.Message.EqualTo("Can't add article to favorite"), async delegate { await followService.AddArticleToFavorite(inputTitle, inputId, inputCurrentUserId); });
        }
    }
}
