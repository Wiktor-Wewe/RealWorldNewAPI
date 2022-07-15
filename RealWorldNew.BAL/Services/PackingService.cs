using RealWorldNew.Common;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.BAL.Services
{
    public class PackingService : IPackingService
    {
        public PackingService()
        {

        }

        public UserResponseContainer PackUser(User user, string Token)
        {
            var userResponse = new UserResponse()
            {
                email = user.Email,
                username = user.UserName,
                bio = user.ShortBio,
                image = user.UrlProfile,
                token = Token
            };

            var userResponseContainer = new UserResponseContainer();
            userResponseContainer.user = userResponse;

            return userResponseContainer;
        }

        public User UnpackRegisterUser(RegisterUserPack pack)
        {
            User user = new User()
            {
                Email = pack.user.email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = pack.user.username
            };

            return user;
        }

        public ProfileView PackUserToProfileView(User user, bool isFollowed)
        {
            ProfileView packProfile = new ProfileView()
            {
                username = user.UserName,
                bio = user.ShortBio,
                image = user.UrlProfile,
                following = isFollowed
            };
            return packProfile;
        }

        public ProfileViewContainer PackProfileView(ProfileView profile)
        {
            var container = new ProfileViewContainer()
            {
                profile = profile
            };
            return container;
        }
    }
}
