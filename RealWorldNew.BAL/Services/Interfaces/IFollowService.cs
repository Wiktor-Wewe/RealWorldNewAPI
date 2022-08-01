using RealWorldNew.Common.DtoModels;
using RealWorldNew.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common
{
    public interface IFollowService
    {
        Task<ProfileViewContainer> FollowUser(string activeUserId, string usernameToFollow);
        Task<ProfileViewContainer> UnfollowUser(string activeUserId, string usernameToFollow);
        Task<articleAUP> AddArticleToFavorite(string title, int id, string currentUserId);
        Task<articleAUP> RemoveArticleFromFavorite(string title, int id, string currentUserId);
    }
}
