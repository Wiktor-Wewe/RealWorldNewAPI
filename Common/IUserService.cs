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
        string GetToken(User user);
        Task AddUser(RegisterUserDto userDto);
        Task<UserResponseContainer> GetMyInfo(ClaimsPrincipal User);
        Task ChangeUser(User user, ChangeProfile settings);
        Task<bool> IsFollowedUser(string id, string username);
        Task<ProfileViewContainer> FollowUser(User user, User userToFollow);
    }
}
