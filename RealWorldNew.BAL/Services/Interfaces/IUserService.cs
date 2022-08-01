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
        Task UpdateUser(string id, ChangeProfile settings);
        Task<ProfileViewContainer> LoadProfile(string username, string id);
    }
}
