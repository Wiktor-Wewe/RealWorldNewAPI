using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RealWorldNew.DAL.Interfaces
{
    public interface IUserRepositorie
    {
        Task<User> GetById(string Id);
        Task AddUser(User user);
        Task ChangeUser(string Id, string username, string bio, string image, string email, string password);
        Task<bool> IsFollowedUser(User user, string username);
        Task AddFollow(User user, User userToFollow);
        Task SaveChangesAsync();
    }
}
