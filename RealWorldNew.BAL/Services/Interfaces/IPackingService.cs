using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common
{
    public interface IPackingService
    {
        UserResponseContainer PackUser(User user, string Token);
        User UnpackRegisterUser(RegisterUserPack pack);
        ProfileView PackUserToProfileView(User user, bool isFollowed);
        ProfileViewContainer PackProfileView(ProfileView profile);
    }
}
