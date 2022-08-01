using RealWorldNew.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common
{
    public interface ICommentService
    {
        Task<CommentPack> AddCommnetAsync(CommentPack pack, string CurrentUserId, string title, int id);
        Task<CommentPack> GetCommentsAsync(string title, int id);
        Task DeleteCommentAsync(string title, int id, int commentId);
    }
}
