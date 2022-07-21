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

namespace RealWorldNewAPI.Test.UserServiceTests
{
    [Category("UpdateUser")]
    public class UpdateUserTest
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task UpdateUser_WithCorrectUserAndSuccessfulUpdate()
        {
            //Arrange
            string inputId = "someId";

            var inputSettings = new ChangeProfile()
            {
                bio = "some bio",
                email = "some email",
                image = "some link",
                password = "some password",
                username = "some username"
            };

            var user = new User()
            {
                ShortBio = "some old bio",
                Email = "some old email",
                UrlProfile = "some old link",
                UserName = "some old username"
            };

            string hashedPassword = "hashed password";

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            var passwordHasher = new Mock<IPasswordHasher<User>>();
            passwordHasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(hashedPassword);

            var userService = new UserService(null, passwordHasher.Object, userManager.Object, null, null);

            //Act
            userService.UpdateUser(inputId, inputSettings);

            //Arrange
            Assert.That(user.Email, Is.EqualTo(inputSettings.email));
            Assert.That(user.UrlProfile, Is.EqualTo(inputSettings.image));
            Assert.That(user.ShortBio, Is.EqualTo(inputSettings.bio));
            Assert.That(user.UserName, Is.EqualTo(inputSettings.username));
        }

        [Test]
        public async Task UserUpdate_WithCorrectUserAndFailedUpdate_ThrowUserException()
        {
            //Arrange
            string inputId = "someId";

            var inputSettings = new ChangeProfile()
            {
                bio = "some bio",
                email = "some email",
                image = "some link",
                password = "some password",
                username = "some username"
            };

            var user = new User()
            {
                ShortBio = "some old bio",
                Email = "some old email",
                UrlProfile = "some old link",
                UserName = "some old username"
            };

            string hashedPassword = "hashed password";

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            var passwordHasher = new Mock<IPasswordHasher<User>>();
            passwordHasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(hashedPassword);

            var logger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, passwordHasher.Object, userManager.Object, null, logger.Object);

            //Arrange and Act
            Assert.ThrowsAsync(Is.TypeOf<UserException>().And.Message.EqualTo("Can't update user"), async delegate { await userService.UpdateUser(inputId, inputSettings); });
        }

        [Test]
        public async Task UpdateUser_WithNullUser_ThrowUserException()
        {
            //Arrange
            string inputId = "someId";

            var inputSettings = new ChangeProfile()
            {
                bio = "some bio",
                email = "some email",
                image = "some link",
                password = "some password",
                username = "some username"
            };

            var user = new User()
            {
                ShortBio = "some old bio",
                Email = "some old email",
                UrlProfile = "some old link",
                UserName = "some old username"
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(default(User));

            var logger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, userManager.Object, null, logger.Object);

            //Arrange and Act
            Assert.ThrowsAsync(Is.TypeOf<UserException>().And.Message.EqualTo("No user found logged in."), async delegate { await userService.UpdateUser(inputId, inputSettings); });
        }
    }
}
