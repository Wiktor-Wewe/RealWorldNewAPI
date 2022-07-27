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
    public class ArticleService : IArticleService
    {
        private readonly UserManager<User> _userManager;
        private readonly IArticleRepositories _articleRepositories;

        public ArticleService(UserManager<User> userManager, IArticleRepositories articleRepositories)
        {
            _userManager = userManager;
            _articleRepositories = articleRepositories;
        }

        public async Task<ArticleUploadResponse> AddArticle(string userId, ArticleUpload pack)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var tags = await _articleRepositories.CheckTags(pack.tagList);

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
                    favorited = user.LikedArticles.Contains(article),
                    favoritesCount = article.NumberOfLikes,
                    author = new authorAUP()
                    {
                        username = article.Author.UserName,
                        bio = article.Author.ShortBio,
                        image = article.Author.UrlProfile,
                        following = user.FollowedUsers.Contains(article.Author)
                    }
                },
            };
            return pack;
        }

        public async Task<List<articleAUP>> articleListToAUP(List<Article> articles, string currentUserId)
        {
            var aup = new List<articleAUP>();
            var user = await _userManager.FindByIdAsync(currentUserId);

            foreach (var article in articles)
            {
                aup.Add(new articleAUP()
                {
                    slug = article.Slug,
                    title = article.Title,
                    description = article.Topic,
                    body = article.Text,
                    tagList = article.Tags.Select(x => x.Name).ToList(),
                    createdAt = article.CreateDate,
                    updatedAt = article.UpdateDate,
                    favorited = user.LikedArticles.Contains(article),
                    favoritesCount = article.NumberOfLikes,
                    author = new authorAUP()
                    {
                        username = article.Author.UserName,
                        bio = article.Author.ShortBio,
                        image = article.Author.UrlProfile,
                        following = user.FollowedUsers.Contains(article.Author)
                    }
                });
            }

            return aup;
        }

        public async Task<MultiArticleResponse> GetArticles(string tag, string favorited, string author, int limit, int offset, string currentUserId)
        {
            var articles = new List<Article>();
            if (tag != null)
            {
                articles = await _articleRepositories.GetArticlesByTagAsync(tag, limit, offset);
            }
            else if (favorited != null)
            {
                articles = await _articleRepositories.GetArticlesByFavoritesAsync(favorited, limit, offset);
            }
            else if(author != null)
            {
                articles = await _articleRepositories.GetArticlesByAuthorAsync(author, limit, offset);
            }
            else
            {
                articles = await _articleRepositories.GetArticlesAsync(limit, offset);
            }

            var pack = new MultiArticleResponse()
            {
                articles = articleListToAUP(articles, currentUserId).Result,
                articlesCount = await _articleRepositories.GetArticlesCount(), //do poprawy
            };

            return pack;
        }

        public async Task<MultiArticleResponse> GetArticlesFeed(int limit, int offset, string currentUserId)
        {
            var articles = await _articleRepositories.GetNewArticleFeed(currentUserId, limit, offset);

            var pack = new MultiArticleResponse()
            {
                articles = articleListToAUP(articles, currentUserId).Result,
                articlesCount = await _articleRepositories.GetArticlesFeedCount(currentUserId)
            };

            return pack;
        }

        public async Task DeleteArticleAsync(string title, int id)
        {
            var artice = await _articleRepositories.FindBySlugAsync(title, id);
            await _articleRepositories.DeleteArticleAsync(artice);
        }

        public async Task<ArticleUploadResponse> EditArticleAsync(ArticleUploadResponse pack, string title, int id)
        {
            var article = await _articleRepositories.FindBySlugAsync(title, id);

            article.Title = pack.article.title;
            article.Topic = pack.article.description;
            article.Text = pack.article.body;
            article.Tags = await _articleRepositories.CheckTags(pack.article.tagList);

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
    }
}
