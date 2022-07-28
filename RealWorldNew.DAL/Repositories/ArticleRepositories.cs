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
                .Include(u => u.Tags)
                .FirstOrDefaultAsync(x => x.Id == id && x.Title == title);

            return article;
        }

        public async Task<List<Article>> GetArticlesByTagAsync(string tag, int limit, int offset)
        {
            var articles = await _dbContext.Tag
                .Include(u => u.Articles)
                    .ThenInclude(u => u.Author)
                .Where(u => u.Name == tag)
                .SelectMany(u => u.Articles)
                .OrderByDescending(u => u.CreateDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();     

            return articles;
        }

        public async Task<List<Article>> GetArticlesByFavoritesAsync(string favorited, int limit, int offset)
        {
            var articles = await _dbContext.Users
                .Include(fu => fu.LikedArticles)
                    .ThenInclude(fa => fa.Author)
                .Where(x => x.UserName == favorited)
                .SelectMany(u => u.LikedArticles)
                .OrderByDescending(u => u.CreateDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return articles;
        }

        public async Task<List<Article>> GetArticlesByAuthorAsync(string author, int limit, int offset)
        {
            var articles = await _dbContext.Article
                    .Include(u => u.Author)
                    .Include(u => u.Tags)
                    .Where(u => u.Author.UserName == author)
                    .OrderByDescending(u => u.CreateDate)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

            return articles;
        }

        public async Task<List<Article>> GetArticlesAsync(int limit, int offset)
        {
            var articles = await _dbContext.Article
                    .Include(u => u.Author)
                    .Include(u => u.Tags)
                    .OrderByDescending(u => u.CreateDate)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

            return articles;
        }

        public async Task<List<Article>> GetNewArticleFeed(string currentUserId, int limit, int offset)
        {

            var result =
                await _dbContext.Users
                .Include(fu => fu.FollowedUsers)
                    .ThenInclude(fa => fa.Articles)
                        .ThenInclude(faa => faa.Author)
                .Where(x => x.Id == currentUserId)
                .SelectMany(u => u.FollowedUsers.SelectMany(a => a.Articles))
                .OrderByDescending(u => u.CreateDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return result;
        }

        public async Task DeleteArticleAsync(Article artice)
        {
            _dbContext.Article.Remove(artice);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditArticleAsync(Article article)
        {
            _dbContext.Article.Update(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddNewTag(string name)
        {
            var tag = new Tag()
            {
                Name = name
            };

            await _dbContext.Tag.AddAsync(tag);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _dbContext.Tag.ToListAsync();
        }

        public async Task<Tag> GetTagByNameAsync(string name)
        {
            return await _dbContext.Tag.FirstOrDefaultAsync(x => x.Name == name);
        }
        
        public async Task<List<Tag>> GetPopularTags()
        {
            var Tags = await _dbContext.Tag
                .Include(u => u.Articles)
                .OrderByDescending(u => u.Articles.Count)
                .ToListAsync();

            return Tags.Take(6).ToList();
        }

        public async Task<int> GetArticlesCount()
        {
            return await _dbContext.Article.CountAsync();
        }

        public async Task<int> GetArticlesFeedCount(string currentUserId)
        {
            var result =
                await _dbContext.Users
                .Include(fu => fu.FollowedUsers)
                    .ThenInclude(fa => fa.Articles)
                .Where(x => x.Id == currentUserId)
                .SelectMany(u => u.FollowedUsers.SelectMany(a => a.Articles))
                .OrderByDescending(u => u.CreateDate)
                .CountAsync();

            return result;
        }

        public async Task<int> GetArticlesCountByTagsAsync(string tag)
        {
            var articles = await _dbContext.Tag
                .Include(u => u.Articles)
                    .ThenInclude(u => u.Author)
                .Where(u => u.Name == tag)
                .SelectMany(u => u.Articles)
                .CountAsync();

            return articles;
        }

        public async Task<int> GetArticlesCountByFavoritesAsync(string favorited)
        {
            var articles = await _dbContext.Users
                .Include(fu => fu.LikedArticles)
                    .ThenInclude(fa => fa.Author)
                .Where(x => x.UserName == favorited)
                .SelectMany(u => u.LikedArticles)
                .CountAsync();

            return articles;
        }

        public async Task<int> GetArticlesCountByAuthorAsync(string author)
        {
            var articles = await _dbContext.Article
                    .Include(u => u.Author)
                    .Include(u => u.Tags)
                    .Where(u => u.Author.UserName == author)
                    .CountAsync();

            return articles;
        }

    }
    
}
