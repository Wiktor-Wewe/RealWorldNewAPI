using Microsoft.AspNetCore.Identity;
using RealWorldNew.Common;
using RealWorldNew.Common.Models;
using RealWorldNew.DAL.Entities;
using RealWorldNew.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.BAL.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepositories _commentRepositories;
        private readonly UserManager<User> _userManager;

        public CommentService(ICommentRepositories commentRepositories, UserManager<User> userManager)
        {
            _commentRepositories = commentRepositories;
            _userManager = userManager;
        }

        public async Task<CommentPack> AddCommnetAsync(CommentPack pack, string CurrentUserId, string title, int id)
        {
            var author = await _userManager.FindByIdAsync(CurrentUserId);

            pack.Comment.CreatedAt = DateTime.Now;
            pack.Comment.UpdatedAt = DateTime.Now;
            pack.Comment.Author = new authorAUP()
            {
                username = author.UserName,
                bio = author.ShortBio,
                image = author.UrlProfile,
                following = false
            };

            Comment comment = new Comment()
            {
                Text = pack.Comment.Body,
                UserId = CurrentUserId,
                CreateDate = pack.Comment.CreatedAt
            };

            await _commentRepositories.AddCommentAsync(comment, title, id);
            return pack;
        }

        public authorAUP UserToAuthorAUP(User user)
        {
            var author = new authorAUP()
            {
                bio = user.ShortBio,
                following = true,
                image = user.UrlProfile,
                username = user.UserName
            };

            return author;
        }

        public async Task<CommentPack> GetCommentsAsync(string title, int id)
        {
            var RawComments = await _commentRepositories.GetCommentsAsync(title, id);

            var comments = new List<CommentView>();
            

            foreach(var comment in RawComments)
            {
                var buffor = new CommentView();
                buffor.Id = comment.Id;
                buffor.CreatedAt = comment.CreateDate;
                buffor.UpdatedAt = comment.CreateDate; //??
                buffor.Body = comment.Text;
                buffor.Author = UserToAuthorAUP(await _userManager.FindByIdAsync(comment.UserId));

                comments.Add(buffor);
            }

            var result = new CommentPack()
            {
                Comments = comments
            };

            return result;
        }

        public async Task DeleteCommentAsync(string title, int id, int commentId)
        {
            await _commentRepositories.DeleteCommentAsync(title, id, commentId);
        }
    }
}
