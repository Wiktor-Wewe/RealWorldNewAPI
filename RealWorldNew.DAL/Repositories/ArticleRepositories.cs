using Microsoft.AspNetCore.Mvc;
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
    public class ArticleRepositories : IArticleRepositories
    {
        private ApplicationDbContext _dbContext;

        public ArticleRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddArticle(Article article)
        {
            _dbContext.Article.Add(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Article> FindBySlugAsync(string title, int id)
        {
            var article = await _dbContext.Article
                .Include(u => u.Author)
                .FirstOrDefaultAsync(x => x.Id == id && x.Title == title);

            return article;
        }

        public async Task<List<Article>> GetNewArticles(string author, int limit) //add favorites
        {
            var articles = new List<Article>();
            if (author != null)
            {
                articles = await _dbContext.Article
                    .Include(u => u.Author)
                    .Where(u => u.Author.UserName == author)
                    .Take(limit)
                    .OrderByDescending(u => u.CreateDate)
                    .ToListAsync();
            }
            else
            {
                articles = await _dbContext.Article
                    .Include(u => u.Author)
                    .Take(limit)
                    .OrderByDescending(u => u.CreateDate)
                    .ToListAsync();
            }
            return articles;
        }

        public async Task<List<Article>> GetNewArticleFeed(string currentUserId, int limit)
        {
            var user = await _dbContext.User
                .Include(u => u.FollowedUsers)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            
            var followerdUsers = user.FollowedUsers
                .ToList();
            
            var allArticles = await _dbContext.Article
                .Include(u => u.Author)
                .OrderByDescending(u => u.CreateDate)
                .ToListAsync();
            
            var articles = new List<Article>();
            foreach(var article in allArticles)
            {
                foreach(var follower in followerdUsers)
                {
                    if(article.Author == follower)
                    {
                        articles.Add(article);
                    }
                }
                if(articles.Count > limit)
                {
                    break;
                }
            }

            return articles;
        }
    }
}
