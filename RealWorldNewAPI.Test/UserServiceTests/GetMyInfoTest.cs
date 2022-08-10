using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL.Services;
using RealWorldNew.Common.Exceptions;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNewAPI.Test.UserServiceTests
{
    [Category("GetMyInfo")]
    public class GetMyInfoTest
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task GetMyInfo_WithCorrectUser_ReturnUserResponseContainer()
        {
            //Arrange
            string input = "someid";

            var user = new User()
            {
                UserName = "name",
                Email = "name@wp.pl"
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var userService = new UserService(null, null, userManager.Object, null, null);

            //Act
            var result = userService.GetMyInfo(input);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(result.Result.user.username, Is.EqualTo(user.UserName));
            Assert.That(result.Result.user.email, Is.EqualTo(user.Email));
        }

        [Test]
        public async Task GetMyInfo_UserNotFound_ReturnNull()
        {
            //Arrange
            string input = "someid";

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(default(User));

            var logger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, userManager.Object, null, logger.Object);

            //Act 
            var response = await userService.GetMyInfo(input);

            //Arrange and Act
            Assert.IsNull(response);
        }
    }
}
