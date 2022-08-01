using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.Common.Exceptions;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNewAPI.Test.FollowServiceTests
{
    [Category("FollowUser")]
    public class FollowUserTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task FollowUser_WithEveryDataOkay_ReturnProfileViweContainer()
        {
            //Arrange
            var activeUserId = "some id";
            var usernameToFollow = "other id";

            var userToFollow = new User()
            {
                UserName = "asd",
                Email = "asd"
            };

            var ActiveUser = new User()
            {
                UserName = "someName",
                Email = "someEmail@wp.pl"
            };

            var response = new ProfileViewContainer()
            {
                profile = new ProfileView()
                {
                    username = userToFollow.UserName,
                    bio = userToFollow.ShortBio,
                    image = userToFollow.UrlProfile,
                    following = true
                }
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(ActiveUser);
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(userToFollow);

            var FollowService = new FollowService(userManager.Object, null, null);

            //Act
            var result = await FollowService.FollowUser(activeUserId, usernameToFollow);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.profile.username, Is.EqualTo(userToFollow.UserName));
        }

        [Test]
        public async Task FollowUser_ActiveUserNotFound_ThrowFollowException()
        {
            //Arrange
            var activeUserId = "some id";

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var logger = new Mock<ILogger<FollowService>>();
            var FollowService = new FollowService(userManager.Object, null, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<FollowException>().And.Message.EqualTo("Can't find active user"), async delegate { await FollowService.FollowUser(activeUserId, "some name"); });
        }

        [Test]
        public async Task FollowUser_ActiveUserAlreadyContainUserToFollow_ThrowFollowException()
        {
            //Arrange
            var activeUserId = "some id";
            var usernameToFollow = "other id";

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
                    userToFollow
                }
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(ActiveUser);
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(userToFollow);

            var logger = new Mock<ILogger<FollowService>>();
            var FollowService = new FollowService(userManager.Object, null, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<FollowException>().And.Message.EqualTo("Active user already contain userToFollow"), async delegate { await FollowService.FollowUser(activeUserId, usernameToFollow); });
        }
    }
}
