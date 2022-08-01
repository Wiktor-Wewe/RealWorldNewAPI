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

namespace RealWorldNewAPI.Test.CommentServiceTests
{
    [Category("AddCommentAsync")]
    public class AddCommentAsyncTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task AddCommentAsync_WithCorrectData_ReturnCommentPack()
        {
            //Arrange 
            var pack = new CommentPack()
            {
                Comment = new CommentView()
                {
                    Author = new authorAUP()
                    {
                        bio = "some bio",
                        following = true,
                        image = "some image",
                        username = "someuser"
                    },
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Body = "body"
                }
            };

            var user = new User(); 

            var currentUserId = "some id";
            var title = "some title";
            var id = 12;

            var commentRepositories = new Mock<ICommentRepositories>();
            commentRepositories.Setup(x => x.AddCommentAsync(It.IsAny<Comment>(), title, id)).ReturnsAsync(id);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(currentUserId)).ReturnsAsync(user);

            var commentService = new CommentService(commentRepositories.Object, userManager.Object, null);

            //Act
            var idResult = commentService.AddCommnetAsync(pack, currentUserId, title, id);

            //Assert
            Assert.IsNotNull(idResult);
        }

        [Test]
        public async Task AddCommentAsync_WhenCantFindAuthor_ThrowCommentException()
        {
            //Arrange 
            var pack = new CommentPack()
            {
                Comment = new CommentView()
                {
                    Author = new authorAUP()
                    {
                        bio = "some bio",
                        following = true,
                        image = "some image",
                        username = "someuser"
                    },
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Body = "body"
                }
            };

            var currentUserId = "some id";
            var title = "some title";
            var id = 12;

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var logger = new Mock<ILogger<CommentService>>();

            var commentService = new CommentService(null, userManager.Object, logger.Object);

            //Assert and Act
            Assert.ThrowsAsync(Is.TypeOf<CommentException>().And.Message.EqualTo("Can't find author of the comment"), async delegate { await commentService.AddCommnetAsync(pack, currentUserId, title, id); });
        }
    }
}
