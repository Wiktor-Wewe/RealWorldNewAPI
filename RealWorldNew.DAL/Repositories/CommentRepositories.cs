using Microsoft.EntityFrameworkCore;
using RealWorldNew.DAL.Entities;
using RealWorldNew.DAL.Interfaces;
using RealWorldNewAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.DAL.Repositories
{
    public class CommentRepositories : ICommentRepositories
    {
        private readonly ApplicationDbContext _dbContext;
        public CommentRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCommentAsync(Comment comment, string title, int id)
        {
            var article = await _dbContext.Article
                .FirstOrDefaultAsync(x => x.Title == title && x.Id == id);

            article.Comments.Add(comment);
            _dbContext.Article.Update(article);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetCommentsAsync(string title, int id)
        {
            var article = await _dbContext.Article
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Title == title && x.Id == id);

            var comments = article.Comments.
                OrderByDescending(x => x.CreateDate)
                .ToList();

            return comments;
        }

        public async Task DeleteCommentAsync(string title, int id, int commentId)
        {
            var article = await _dbContext.Article
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Title == title && x.Id == id);

            var comment = await _dbContext.Comment
                .FirstOrDefaultAsync(x => x.Id == commentId);

            article.Comments.Remove(comment);

            _dbContext.Comment.Remove(comment);
            _dbContext.Article.Update(article);
            await _dbContext.SaveChangesAsync();
        }
    }
}
