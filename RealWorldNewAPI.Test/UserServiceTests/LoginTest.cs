using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.Common.Exceptions;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealWorldNew.BAL.Services;
using RealWorldNewAPI.Test.UserServiceTests;

namespace RealWorldNewAPI.Test.UserServiceTests
{
    [Category("Login")]
    public class LoginTest
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task Login_WithCorrectData()
        {
            //Arrange
            LoginUserPack input = new LoginUserPack()
            {
                user = new LoginUser
                {
                    Email = "qwe123@wp.pl",
                    Password = "ZAQ!2wsx"
                }
            };

            User output = new User()
            {
                Email = "qwe123@wp.pl"
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(output);
            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            var userService = new UserService(null, null, userManager.Object, null, null);

            //Act
            var result = userService.Login(input);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.Result.Email, Is.EqualTo(output.Email));
            Assert.That(result.Result.PasswordHash, Is.EqualTo(output.PasswordHash));
        }

        [Test]
        public async Task Login_UserIsNull_ThrowUserException()
        {
            //Arrange
            LoginUserPack input = new LoginUserPack()
            {
                user = new LoginUser
                {
                    Email = "qwe123@wp.pl",
                    Password = "ZAQ!2wsx"
                }
            };

            User output = new User()
            {
                Email = "qwe123@wp.pl"
            };


            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var logger = new Mock<ILogger<UserService>>();
            var userService = new UserService(null, null, userManager.Object, null, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<UserException>().And.Message.EqualTo("Invalid username or password."), async delegate { await userService.Login(input); });
        }

        [Test]
        public async Task Login_WrongPassword_ThrowUserException()
        {
            //Arrange
            LoginUserPack input = new LoginUserPack()
            {
                user = new LoginUser
                {
                    Email = "qwe123@wp.pl",
                    Password = "ZAQ!2wsx"
                }
            };

            User output = new User()
            {
                Email = "qwe123@wp.pl"
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(output);
            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

            var logger = new Mock<ILogger<UserService>>();
            var userService = new UserService(null, null, userManager.Object, null, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<UserException>().And.Message.EqualTo("Invalid username or password."), async delegate { await userService.Login(input); });
        }
    }
}
