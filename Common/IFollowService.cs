using RealWorldNew.Common.DtoModels;
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
    }
}
