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

        public async Task<List<Article>> GetNewArticles(string favorited, string author, int limit)
        {
            var articles = new List<Article>();
            if (favorited != null)
            {
                var user = await _dbContext.User
                    .Include(u => u.LikedArticles)
                    .FirstOrDefaultAsync(u => u.UserName == favorited);

                var list = user.LikedArticles.ToList();

                var allArticles = await _dbContext.Article.Include(u => u.Author).ToListAsync();

                foreach(var article in allArticles)
                {
                    if (list.Contains(article))
                    {
                        articles.Add(article);
                    }
                    
                    if(articles.Count >= limit)
                    {
                        break;
                    }
                }
            }
            else if (author != null)
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

        public async Task<List<Tag>> CheckTags(List<String> tagsNames)
        {
            var tagsInDb = await _dbContext.Tag.ToListAsync();

            foreach(var tag in tagsNames)
            {
                if(tagsInDb.FirstOrDefault<Tag>(x => x.Name == tag) == null)
                {
                    await AddNewTag(tag);
                }
            }

            var tagList = new List<Tag>();
            Tag buff;

            foreach(var tag in tagsNames)
            {
                buff = await _dbContext.Tag.FirstOrDefaultAsync(x => x.Name == tag);
                tagList.Add(buff);
            }

            return tagList;
        }

        public async Task AddArticlesToTags(Article article, List<Tag> tags)
        {
            foreach(var tag in tags)
            {
                tag.Articles.Add(article);
                _dbContext.Update(tag);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetPopularTags()
        {
            var Tags = await _dbContext.Tag.Include(u => u.Articles).OrderBy(u => u.Articles.Count).ToListAsync();
            return Tags.Take(6).ToList();
        }
    }
}
