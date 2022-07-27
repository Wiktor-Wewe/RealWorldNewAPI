using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.DAL.Interfaces
{
    public interface ICommentRepositories
    {
        Task AddCommentAsync(Comment comment, string title, int id);
        Task<List<Comment>> GetCommentsAsync(string title, int id);
        Task DeleteCommentAsync(string title, int id, int commentId);
    }
}
