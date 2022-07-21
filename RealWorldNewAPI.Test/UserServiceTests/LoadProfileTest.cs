using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.Common.Exceptions;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNewAPI.Test.UserServiceTests
{
    [Category("LoadProfile")]
    public class LoadProfileTest
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task LoadProfile_FoundUser_ReturnProfileViewContainer()
        {
            //Arrange
            string inputUsername = "some username";
            string inputId = "some id";

            var user = new User()
            {
                UserName = "some username",
                ShortBio = "some bio",
                UrlProfile = "some link"
            };

            var profile = new ProfileView()
            {
                username = "some username",
                bio = "some bio",
                image = "some link",
                following = false,
            };

            var output = new ProfileViewContainer()
            {
                profile = profile
            };

            var userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()).Result).Returns(user);
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()).Result).Returns(user);

            var packingService = new Mock<IPackingService>();
            packingService.Setup(x => x.PackUserToProfileView(It.IsAny<User>(), It.IsAny<bool>())).Returns(profile);
            packingService.Setup(x => x.PackProfileView(It.IsAny<ProfileView>())).Returns(output);

            var userService = new UserService(null, null, userManager.Object, packingService.Object, null);

            //Act
            var result = userService.LoadProfile(inputUsername, inputId);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.Result.profile.username, Is.EqualTo(inputUsername));
        }

        [Test]
        public async Task LoadProfile_UserIsNull_ThrowUserException()
        {
            //Arrange
            string inputUsername = "some username";

            var userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()).Result).Returns(default(User));

            var logger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, userManager.Object, null, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<UserException>().And.Message.EqualTo($"Can not find user {inputUsername}"), async delegate { await userService.LoadProfile(inputUsername, "some id"); });
        }
    }
}
