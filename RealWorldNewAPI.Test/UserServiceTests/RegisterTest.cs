using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.Common.Exceptions;
using RealWorldNew.DAL.Entities;

namespace RealWorldNewAPI.Test.UserServiceTests
{
    [Category("Register")]
    public class RegisterTest
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        
        [Test]
        public async Task Register_WithCorrectData_ReturnUser()
        {
            //Arrange
            var input = new RegisterUserPack()
            {
                user = new RegisterUserDto()
                {
                    username = "name",
                    email = "email@wp.pl",
                    password = "password"
                }
            };

            var output = new User()
            {
                UserName = "name",
                Email = "email@wp.pl",
            };

            var mockPackingService = new Mock<IPackingService>();
            mockPackingService.Setup(x => x.UnpackRegisterUser(It.IsAny<RegisterUserPack>())).Returns(output);

            var userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(default(User));
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(null, null, userManager.Object, mockPackingService.Object, null);

            //Act
            var result = userService.Register(input);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.Result.UserName, Is.EqualTo(output.UserName));
            Assert.That(result.Result.Email, Is.EqualTo(output.Email));
        }

        [Test]
        public async Task Register_WithExistingUser_ThrowUserException()
        {
            //Arrange
            var input = new RegisterUserPack()
            {
                user = new RegisterUserDto()
                {
                    username = "name",
                    email = "email@wp.pl",
                    password = "password"
                }
            };

            var output = new User()
            {
                UserName = "name",
                Email = "email@wp.pl",
            };

            var userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(output);

            var logger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, userManager.Object, null, logger.Object);


            //Arrange and act
            Assert.ThrowsAsync(Is.TypeOf<UserException>().And.Message.EqualTo("Username already taken."), async delegate { await userService.Register(input); });
        }

        [Test]
        public async Task Register_CantCreateUser_ThrowUserException()
        {
            //Arrange
            var input = new RegisterUserPack()
            {
                user = new RegisterUserDto()
                {
                    username = "name",
                    email = "email@wp.pl",
                    password = "password"
                }
            };

            var output = new User()
            {
                UserName = "name",
                Email = "email@wp.pl",
            };

            var mockPackingService = new Mock<IPackingService>();
            mockPackingService.Setup(x => x.UnpackRegisterUser(It.IsAny<RegisterUserPack>())).Returns(output);

            var userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var logger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, userManager.Object, mockPackingService.Object, logger.Object);

            //Arrange and act
            Assert.ThrowsAsync(Is.TypeOf<UserException>().And.Message.EqualTo("Wrong input"), async delegate { await userService.Register(input); });
        }
    }
}