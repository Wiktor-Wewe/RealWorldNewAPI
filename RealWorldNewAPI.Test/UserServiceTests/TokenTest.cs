using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldNew.BAL;
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
    [Category("Token")]
    public class TokenTest
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public void GetToken_WithUser_ReturnToken()
        {
            //Arrange
            var user = new User()
            {
                UserName = "jajomajo123",
                Email = "rybawmajonezie321@gmail.com",
            };

            AuthenticationSettings authenticationSettings = new AuthenticationSettings
            {
                JwtKey = "PRIVATE_KEY_DONT_SHARE",
                JwtExpireDays = 5,
                JwtIssuer = "http://localhost:47765"
            };

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            var userService = new UserService(authenticationSettings, null, mockUserManager.Object, null, null);

            //Act
            var actual = userService.GetToken(user);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetToken_WithNullUser_ThrowUserException()
        {
            //Arrange
            var logger = new Mock<ILogger<UserService>>();
            var userService = new UserService(null, null, null, null, logger.Object);

            //Assert and Act
            Assert.Throws(Is.TypeOf<UserException>().And.Message.EqualTo("Can't generate token"), delegate { userService.GetToken(default(User)); });
        }
    }
}
