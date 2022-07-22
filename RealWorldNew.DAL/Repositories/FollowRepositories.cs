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
    public class FollowRepositories : IFollowRepositories
    {
        private readonly ApplicationDbContext _dbContext;
        public FollowRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Article> GetArticleBySlugAsync(string title, int id, bool like)
        {
            var article = await _dbContext.Article
                .Include(u => u.Author)
                .FirstOrDefaultAsync(u => u.Title == title && u.Id == id);

            if (like)
            {
                article.NumberOfLikes++;
            }
            else
            {
                article.NumberOfLikes--;
            }
            
            _dbContext.Article.Update(article);
            await _dbContext.SaveChangesAsync();

            return article;
        }
    }
}
