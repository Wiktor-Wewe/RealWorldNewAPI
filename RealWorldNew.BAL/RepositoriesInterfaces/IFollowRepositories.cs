using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.DAL.Interfaces
{
    public interface IFollowRepositories
    {
        Task<Article> GetArticleBySlugAsync(string title, int id, bool like);
    }
}
