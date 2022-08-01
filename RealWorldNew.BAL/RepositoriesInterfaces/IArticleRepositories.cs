using RealWorldNew.DAL.Entities;

namespace RealWorldNew.DAL.Interfaces
{
    public interface IArticleRepositories
    {
        Task AddArticle(Article article);
        Task<Article> FindBySlugAsync(string title, int id);
        Task<List<Article>> GetArticlesByTagAsync(string tag, int limit, int offset);
        Task<List<Article>> GetArticlesByFavoritesAsync(string favorited, int limit, int offset);
        Task<List<Article>> GetArticlesByAuthorAsync(string author, int limit, int offset);
        Task<List<Article>> GetArticlesAsync(int limit, int offset);
        Task<List<Article>> GetNewArticleFeed(string currentUserId, int limit, int offset);
        Task DeleteArticleAsync(Article artice);
        Task EditArticleAsync(Article article);
        Task AddNewTag(string name);
        Task<List<Tag>> GetAllTagsAsync();
        Task<Tag> GetTagByNameAsync(string name);
        Task<List<Tag>> GetPopularTags();
        Task<int> GetArticlesCount();
        Task<int> GetArticlesFeedCount(string currentUserId);
        Task<int> GetArticlesCountByTagsAsync(string tag);
        Task<int> GetArticlesCountByFavoritesAsync(string favorited);
        Task<int> GetArticlesCountByAuthorAsync(string author);
    }
}
