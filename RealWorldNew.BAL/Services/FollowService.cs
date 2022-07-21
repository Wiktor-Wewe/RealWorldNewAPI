using Microsoft.AspNetCore.Identity;
using RealWorldNew.Common;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;
using RealWorldNew.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.BAL.Services
{
    public class FollowService : IFollowService
    {
        private readonly UserManager<User> _userManager;

        public FollowService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ProfileViewContainer> FollowUser(string activeUserId, string usernameToFollow)
        {
            var activeUser = await _userManager.FindByIdAsync(activeUserId);
            if (activeUser == null)
            {
                //log and exception
            }

            var userToFollow = await _userManager.FindByNameAsync(usernameToFollow);

            var response = new ProfileViewContainer()
            {
                profile = new ProfileView()
                {
                    username = userToFollow.UserName,
                    bio = userToFollow.ShortBio,
                    image = userToFollow.UrlProfile,
                    following = true
                }
            };

            if (activeUser.FollowedUsers.Contains(userToFollow))
            {
                //log and exception
            }
            else
            {
                activeUser.FollowedUsers.Add(userToFollow);
                await _userManager.UpdateAsync(activeUser);
                response.profile.following = true;
            }

            return response;
        }

        public async Task<ProfileViewContainer> UnfollowUser(string activeUserId, string usernameToFollow)
        {
            var activeUser = await _userManager.FindByIdAsync(activeUserId);
            if (activeUser == null)
            {
                //log and exception
            }

            var userToFollow = await _userManager.FindByNameAsync(usernameToFollow);

            var response = new ProfileViewContainer()
            {
                profile = new ProfileView()
                {
                    username = userToFollow.UserName,
                    bio = userToFollow.ShortBio,
                    image = userToFollow.UrlProfile,
                    following = false
                }
            };

            if (activeUser.FollowedUsers.Contains(userToFollow))
            {
                activeUser.FollowedUsers.Remove(userToFollow);
                await _userManager.UpdateAsync(activeUser);
            }
            else
            {
                //log and exception
            }

            return response;
        }
    }
}
