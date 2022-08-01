using Microsoft.AspNetCore.Identity;
using Moq;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;
using RealWorldNew.BAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RealWorldNew.Common.Exceptions;

namespace RealWorldNewAPI.Test.FollowServiceTests
{
    [Category("UnfollowUser")]
    public class UnfollowUserTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task UnfollowUser_WithEveryDataOkay_ReturnProfileViewContainer()
        {
            //Arrange
            var activeUserId = "some id";
            var usernameToUnfollow = "other id";

            var userToUnfollow = new User()
            {
                UserName = "asd",
                Email = "asd"
            };

            var ActiveUser = new User()
            {
                UserName = "someName",
                Email = "someEmail@wp.pl",
                FollowedUsers = new List<User>()
                {
                    userToUnfollow
                }
            };

            var response = new ProfileViewContainer()
            {
                profile = new ProfileView()
                {
                    username = userToUnfollow.UserName,
                    bio = userToUnfollow.ShortBio,
                    image = userToUnfollow.UrlProfile,
                    following = true
                }
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(ActiveUser);
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(userToUnfollow);

            var FollowService = new FollowService(userManager.Object, null, null);

            //Act
            var result = await FollowService.UnfollowUser(activeUserId, usernameToUnfollow);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.profile.username, Is.EqualTo(userToUnfollow.UserName));
        }

        [Test]
        public async Task UnfollowUser_ActiveUserNotFound_ThrowFollowException()
        {
            //Arrange
            var activeUserId = "some id";

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var logger = new Mock<ILogger<FollowService>>();
            var FollowService = new FollowService(userManager.Object, null, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<FollowException>().And.Message.EqualTo("Can't find active user"), async delegate { await FollowService.UnfollowUser(activeUserId, "some name"); });
        }

        [Test]
        public async Task UnfollowUser_ActiveUserAlreadyNotContainUserToFollow_ThrowFollowException()
        {
            //Arrange
            var activeUserId = "some id";
            var usernameToUnfollow = "other id";

            var userToFollow = new User()
            {
                UserName = "asd",
                Email = "asd"
            };

            var ActiveUser = new User()
            {
                UserName = "someName",
                Email = "someEmail@wp.pl",
                FollowedUsers = new List<User>()
                {

                }
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(ActiveUser);
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(userToFollow);

            var logger = new Mock<ILogger<FollowService>>();
            var FollowService = new FollowService(userManager.Object, null, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<FollowException>().And.Message.EqualTo("Active user not contains userToFollow"), async delegate { await FollowService.UnfollowUser(activeUserId, usernameToUnfollow); });
        }
    }
}
