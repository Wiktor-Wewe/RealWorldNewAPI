using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IUserService
    {
        Task<User> Register(RegisterUserPack userPack);
        Task<User> Login(LoginUserPack modelPack);
        string GetToken(User user);
        Task<UserResponseContainer> GetMyInfo(string Id);
        Task ChangeUser(User user, ChangeProfile settings);
        Task<bool> IsFollowedUser(string id, string username);
        Task<ProfileViewContainer> FollowUser(User user, User userToFollow);
    }
}
