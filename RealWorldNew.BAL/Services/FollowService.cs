using Microsoft.AspNetCore.Identity;
using RealWorldNew.Common;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.Common.Models;
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
        private readonly IFollowRepositories _followRepositories;

        public FollowService(UserManager<User> userManager, IFollowRepositories followRepositories)
        {
            _userManager = userManager;
            _followRepositories = followRepositories;
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

        public async Task<articleAUP> AddArticleToFavorite(string title, int id, string currentUserId)
        {
            var article = await _followRepositories.GetArticleBySlugAsync(title, id, true);

            var user = await _userManager.FindByIdAsync(currentUserId);
            user.LikedArticles.Add(article);

            var result = await _userManager.UpdateAsync(user);
            //dodaj if result == fail itp

            var pack = new articleAUP()
            {
                slug = article.Slug,
                title = article.Title,
                description = article.Topic,
                body = article.Text,
                tagList = article.Tags.Select(x => x.Name).ToList(), //dodać include tags do articles
                createdAt = article.CreateDate,
                updatedAt =  article.UpdateDate,
                favorited = user.LikedArticles.Contains(article),
                favoritesCount = article.NumberOfLikes,
                author = new authorAUP()
                {
                    username = article.Author.UserName,
                    bio = article.Author.ShortBio,
                    image = article.Author.UrlProfile,
                    following = user.FollowedUsers.Contains(article.Author)
                }
            };

            return pack;
        }

        public async Task<articleAUP> RemoveArticleFromFavorite(string title, int id, string currentUserId)
        {
            var article = await _followRepositories.GetArticleBySlugAsync(title, id, false);

            var user = await _userManager.FindByIdAsync(currentUserId);
            user.LikedArticles.Remove(article);

            var result = await _userManager.UpdateAsync(user);
            //dodaj if result == fail itp

            var pack = new articleAUP()
            {
                slug = article.Slug,
                title = article.Title,
                description = article.Topic,
                body = article.Text,
                tagList = article.Tags.Select(x => x.Name).ToList(), //dodać include tags do articles
                createdAt = article.CreateDate,
                updatedAt = article.UpdateDate,
                favorited = user.LikedArticles.Contains(article),
                favoritesCount = article.NumberOfLikes,
                author = new authorAUP()
                {
                    username = article.Author.UserName,
                    bio = article.Author.ShortBio,
                    image = article.Author.UrlProfile,
                    following = user.FollowedUsers.Contains(article.Author)
                }
            };

            return pack;
        }
    }
}
