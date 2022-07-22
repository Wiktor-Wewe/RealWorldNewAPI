using RealWorldNew.Common.Models;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common
{
    public interface IArticleService
    {
        Task<ArticleUploadResponse> AddArticle(string userId, ArticleUpload pack);
        Task<ArticleUploadResponse> GetArticle(string currentUserId, string title, int id);
        Task<List<articleAUP>> articleListToAUP(List<Article> articles, string currentUserId);
        Task<MultiArticleResponse> GetArticles(string favorited, string author, int limit, int offset, string currentUserId);
        Task<MultiArticleResponse> GetArticlesFeed(int limit, int offset, string currentUserId);
    }
}
