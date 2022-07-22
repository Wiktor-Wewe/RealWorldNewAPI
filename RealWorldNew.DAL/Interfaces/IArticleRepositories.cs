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
        Task<List<Article>> GetNewArticles(string author, int limit);
        Task<List<Article>> GetNewArticleFeed(string currentUserId, int limit);
    }
}
