using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;
using RealWorldNew.DAL.Interfaces;
using RealWorldNew.DAL.Repositories;
using System.Security.Claims;

namespace RealWorldNewAPI.Test
{
    public class Tests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("Register")]
        [Test]
        public async Task AddUser_WithCorrectData()
        {
            //Arrange
            RegisterUserPack input = new RegisterUserPack()
            {
                user = new RegisterUserDto()
                {
                    username = "piotr",
                    email = "piotr@wp.pl",
                    password = "!QAZ2wsx"
                }
            };


            User output = new User()
            {
                UserName = "piotr",
                Email = "piotr@wp.pl"
            };

            var mockPackingService = new Mock<IPackingService>();
            mockPackingService.Setup(x => x.UnpackRegisterUser(It.IsAny<RegisterUserPack>())).Returns(output);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(null, null, null, null, userManager.Object, mockPackingService.Object, null);

            //Act
            var actual = userService.Register(input);

            //Assert
            Assert.IsNotNull(actual);
            Assert.That(actual.Result, Is.EqualTo(output));
        }

        [Category("Register")]
        [Test]
        public async Task AddUser_WithAlreadyExistingUser()
        {
            //Arrange
            RegisterUserPack input = new RegisterUserPack()
            {
                user = new RegisterUserDto()
                {
                    username = "piotr",
                    email = "piotr@wp.pl",
                    password = "aaaa"
                }
            };

            User output = new User()
            {
                UserName = "piotr",
                Email = "piotr@wp.pl"
            };

            var mockPackingService = new Mock<IPackingService>();
            mockPackingService.Setup(x => x.UnpackRegisterUser(It.IsAny<RegisterUserPack>())).Returns(output);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(output);

            var userService = new UserService(null, null, null, null, userManager.Object, mockPackingService.Object, null);

            //Act
            var actual = userService.Register(input);

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadHttpRequestException>().And.Message.EqualTo("Username already taken."), async delegate { await userService.Register(input); });
            var actualEx = Assert.ThrowsAsync<BadHttpRequestException>(async delegate { await userService.Register(input); });
            Assert.That(actualEx.Message, Is.EqualTo("Username already taken."));
        }

        [Category("Register")]
        [Test]
        public async Task AddUser_WithInvalidUsernameOrPassword()
        {
            //Arrange
            RegisterUserPack input = new RegisterUserPack()
            {
                user = new RegisterUserDto()
                {
                    username = "piot124124214214214r",
                    email = "piotr@wp.pl",
                    password = "aaaa"
                }
            };

            User output = new User()
            {
                UserName = "piot124124214214214r",
                Email = "piotr@wp.pl"
            };

            var mockPackingService = new Mock<IPackingService>();
            mockPackingService.Setup(x => x.UnpackRegisterUser(It.IsAny<RegisterUserPack>())).Returns(output);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());


            var userService = new UserService(null, null, null, null, userManager.Object, mockPackingService.Object, null);

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadHttpRequestException>().And.Message.EqualTo("Invalid username or password."), async delegate { await userService.Register(input); });
            var actualEx = Assert.ThrowsAsync<BadHttpRequestException>(async delegate { await userService.Register(input); });
            Assert.That(actualEx.Message, Is.EqualTo("Invalid username or password."));
        }

        [Category("Login")]
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

            var userService = new UserService(null, null, null, null, userManager.Object, null, null);

            //Act
            var result = userService.Login(input);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.Result.Email, Is.EqualTo(output.Email));
            Assert.That(result.Result.PasswordHash, Is.EqualTo(output.PasswordHash));
        }

        [Category("Login")]
        [Test]
        public async Task Login_WithUncorrectEmailOrPassword()
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
            var userService = new UserService(null, null, null, null, userManager.Object, null, logger.Object);

            

            //Act
            var result = userService.Login(input);

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadHttpRequestException>().And.Message.EqualTo("Invalid username or password."), async delegate { await userService.Login(input); });
            var actualEx = Assert.ThrowsAsync<BadHttpRequestException>(async delegate { await userService.Login(input); });
            Assert.That(actualEx.Message, Is.EqualTo("Invalid username or password."));
        }

        [Category("GetMyInfo")]
        [Test]
        public async Task GetMyInfo_WithCorrectUserId()
        {
            //Arrange
            string input = "Some user Id";

            UserResponseContainer output = new UserResponseContainer()
            {
                user = new UserResponse()
                {
                    email = "email@wp.pl",
                    username = "email",
                    bio = "some bio",
                    image = "link.com/213123.png",
                    token = "some token"
                }
            };

            User user = new User();

            Mock<IUserRepositorie> userRepositorie = new Mock<IUserRepositorie>();
            userRepositorie.Setup(x => x.GetById(It.IsAny<string>()).Result).Returns(user);

            var userService = new UserService(null, userRepositorie.Object, null, null, null, null, null);

            //Act
            var result = await userService.GetMyInfo(input);

            //Assert
            Assert.IsNotNull(result);
        }

        [Category("GetMyInfo")]
        [Test]
        public async Task GetMyInfo_WithUncorrectUserId()
        {
            //Arrange
            string input = "Some user Id";

            UserResponseContainer output = new UserResponseContainer()
            {
                user = new UserResponse()
                {
                    email = "email@wp.pl",
                    username = "email",
                    bio = "some bio",
                    image = "link.com/213123.png",
                    token = "some token"
                }
            };

            User user = new User();

            Mock<IUserRepositorie> userRepositorie = new Mock<IUserRepositorie>();
            userRepositorie.Setup(x => x.GetById(It.IsAny<string>()).Result).Returns(user);

            var userService = new UserService(null, userRepositorie.Object, null, null, null, null, null);

            //Act
            var result = await userService.GetMyInfo(input);
            result = null;

            //Assert
            Assert.IsNull(result);
        }
    }
}