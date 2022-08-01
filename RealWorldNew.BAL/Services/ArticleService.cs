using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RealWorldNew.Common;
using RealWorldNew.Common.Exceptions;
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
    public class ArticleService : IArticleService
    {
        private readonly UserManager<User> _userManager;
        private readonly IArticleRepositories _articleRepositories;
        private readonly ILogger _logger;

        public ArticleService(UserManager<User> userManager, IArticleRepositories articleRepositories, ILogger<ArticleService> logger)
        {
            _userManager = userManager;
            _articleRepositories = articleRepositories;
            _logger = logger;
        }

        public async Task<ArticleUploadResponse> AddArticle(string userId, ArticleUpload pack)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                _logger.LogError("Can't find user while adding article");
                throw new ArticleException("Can't find user while adding article");
            }

            var tags = await CheckTagsAsync(pack.tagList);

            var article = new Article()
            {
                Author = user,
                CreateDate = DateTime.Now,
                NumberOfLikes = 0,
                Title = pack.title,
                Topic = pack.description,
                Text = pack.body,
                Tags = tags
            };

            await _articleRepositories.AddArticle(article);

            var resposne = new ArticleUploadResponse()
            {
                article = new articleAUP()
                {
                    slug = article.Slug,
                    title = article.Title,
                    description = article.Topic,
                    body = article.Text,
                    tagList = article.Tags.Select(x => x.Name).ToList(),
                
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now,
                    favorited = false,
                    favoritesCount = 0,
                    author = new authorAUP()
                    {
                        username = user.UserName,
                        bio = user.ShortBio,
                        image = user.UrlProfile,
                        following = false
                    }
                },
            };

            return resposne;
        }

        public async Task<ArticleUploadResponse> GetArticle(string currentUserId, string title, int id)
        {
            var article = await _articleRepositories.FindBySlugAsync(title, id);
            if(article == null)
            {
                _logger.LogError("Can't find article");
                throw new ArticleException("Can't find article");
            }

            var user = await _userManager.FindByIdAsync(currentUserId);

            var pack = new ArticleUploadResponse()
            {
                article = new articleAUP()
                {
                    slug = article.Slug,
                    title = article.Title,
                    description = article.Topic,
                    body = article.Text,
                    tagList = article.Tags.Select(x => x.Name).ToList(),
                    createdAt = article.CreateDate,
                    updatedAt = article.UpdateDate,
                    favorited = user is null ? false : user.LikedArticles.Contains(article),
                    favoritesCount = article.NumberOfLikes,
                    author = new authorAUP()
                    {
                        username = article.Author.UserName,
                        bio = article.Author.ShortBio,
                        image = article.Author.UrlProfile,
                        following = user is null ? false : user.FollowedUsers.Contains(article.Author)
                    }
                },
            };
            return pack;
        }

        public async Task<List<articleAUP>> articleListToAUP(List<Article> articles, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(currentUserId);

            return articles
                .Select(article => new articleAUP()
                {
                    slug = article.Slug,
                    title = article.Title,
                    description = article.Topic,
                    body = article.Text,
                    tagList = article.Tags.Select(x => x.Name).ToList(),
                    createdAt = article.CreateDate,
                    updatedAt = article.UpdateDate,
                    favorited = user is null ? false : user.LikedArticles.Contains(article),
                    favoritesCount = article.NumberOfLikes,
                    author = new authorAUP()
                    {
                        username = article.Author.UserName,
                        bio = article.Author.ShortBio,
                        image = article.Author.UrlProfile,
                        following = user is null ? false : user.FollowedUsers.Contains(article.Author)
                    }
                })
                .ToList();
        }

        public async Task<MultiArticleResponse> GetArticles(string tag, string favorited, string author, int limit, int offset, string currentUserId)
        {
            var articles = new List<Article>();
            int count = 0;
            if (tag != null)
            {
                articles = await _articleRepositories.GetArticlesByTagAsync(tag, limit, offset);
                count = await _articleRepositories.GetArticlesCountByTagsAsync(tag);
            }
            else if (favorited != null)
            {
                articles = await _articleRepositories.GetArticlesByFavoritesAsync(favorited, limit, offset);
                count = await _articleRepositories.GetArticlesCountByFavoritesAsync(favorited);
            }
            else if(author != null)
            {
                articles = await _articleRepositories.GetArticlesByAuthorAsync(author, limit, offset);
                count = await _articleRepositories.GetArticlesCountByAuthorAsync(author);
            }
            else
            {
                articles = await _articleRepositories.GetArticlesAsync(limit, offset);
                count = await _articleRepositories.GetArticlesCount();
            }

            var pack = new MultiArticleResponse()
            {
                articles = await articleListToAUP(articles, currentUserId),
                articlesCount = count
            };

            return pack;
        }

        public async Task<MultiArticleResponse> GetArticlesFeed(int limit, int offset, string currentUserId)
        {
            var articles = await _articleRepositories.GetNewArticleFeed(currentUserId, limit, offset);

            var pack = new MultiArticleResponse()
            {
                articles = await articleListToAUP(articles, currentUserId),
                articlesCount = await _articleRepositories.GetArticlesFeedCount(currentUserId)
            };

            return pack;
        }

        public async Task DeleteArticleAsync(string title, int id)
        {
            var artice = await _articleRepositories.FindBySlugAsync(title, id);
            if(artice == null)
            {
                _logger.LogError("Can't find article while deleting");
                throw new ArticleException("Can't find article while deleting");
            }

            await _articleRepositories.DeleteArticleAsync(artice);
        }

        public async Task<ArticleUploadResponse> EditArticleAsync(ArticleUploadResponse pack, string title, int id)
        {
            var article = await _articleRepositories.FindBySlugAsync(title, id);
            if(article == null)
            {
                _logger.LogError("Can't find article while editing");
                throw new ArticleException("Can't find article while editing");
            }

            article.Title = pack.article.title;
            article.Topic = pack.article.description;
            article.Text = pack.article.body;
            article.Tags = await CheckTagsAsync(pack.article.tagList);

            await _articleRepositories.EditArticleAsync(article);

            pack.article.slug = $"{pack.article.title}-{id}";
            return pack;
        }

        public async Task<PopularTags> GetPopularTags()
        {
            var tags = await _articleRepositories.GetPopularTags();

            var pack = new PopularTags()
            {
                tags = tags.Select(u => u.Name).ToList()
            };

            return pack;
        }

        public async Task<List<Tag>> CheckTagsAsync(List<string> tagsNames)
        {
            var tagsInDb = await _articleRepositories.GetAllTagsAsync();

            foreach (var tag in tagsNames)
            {
                if (tagsInDb.FirstOrDefault<Tag>(x => x.Name == tag) == null)
                {
                    await _articleRepositories.AddNewTag(tag);
                }
            }

            var tagList = new List<Tag>();
            Tag buff;

            foreach (var tag in tagsNames)
            {
                buff = await _articleRepositories.GetTagByNameAsync(tag);
                tagList.Add(buff);
            }

            return tagList;
        } 
    }
}
