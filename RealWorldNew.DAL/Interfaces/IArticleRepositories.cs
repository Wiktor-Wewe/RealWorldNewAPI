using Microsoft.AspNetCore.Mvc;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.DAL.Interfaces
{
    public interface IArticleRepositories
    {
        Task AddArticle(Article article);
        Task<Article> FindBySlugAsync(string title, int id);
        Task<List<Article>> GetNewArticles(string tag, string favorited, string author, int limit);
        Task<List<Article>> GetNewArticleFeed(string currentUserId, int limit);
        Task DeleteArticleAsync(Article artice);
        Task EditArticleAsync(Article article);
        Task AddNewTag(string name);
        Task<List<Tag>> CheckTags(List<String> tagsNames);
        Task AddArticlesToTags(Article article, List<Tag> tags);
        Task<List<Tag>> GetPopularTags();
    }
}
