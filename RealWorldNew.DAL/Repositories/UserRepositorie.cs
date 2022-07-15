using Microsoft.EntityFrameworkCore;
using RealWorldNew.DAL.Entities;
using RealWorldNew.DAL.Interfaces;
using RealWorldNewAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.DAL.Repositories
{
    public class UserRepositorie : IUserRepositorie
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepositorie(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetById(string Id)
        {
            var user = await _dbContext.User.Where(u => u.Id == Id).FirstOrDefaultAsync();
            return user;
        }

        public async Task ChangeUser(string Id, string username, string bio, string image, string email, string password)
        {
            var user = await _dbContext.User.Where(u => u.Id == Id).FirstOrDefaultAsync();

            user.UserName = username;
            user.ShortBio = bio;
            user.UrlProfile = image;
            user.Email = email;
            user.PasswordHash = password;

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddUser(User user)
        {
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsFollowedUser(User user, string username)
        {
            var FollowedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (FollowedUser == null) return false;
            if (user.FollowedUsers != null && user.FollowedUsers.Contains(FollowedUser)) return true;
            return false;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddFollow(User user, User userToFollow)
        {
            var list = user.FollowedUsers.ToList();
            user.FollowedUsers.Add(userToFollow);
            await SaveChangesAsync();
        }
    }
}
