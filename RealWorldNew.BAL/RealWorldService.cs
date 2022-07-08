using Common;
using RealWorldNew.DAL.Entities;

using RealWorldNew.BAL;
using RealWorldNewAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace RealWorldNew.BAL
{
    public class RealWorldService : IRealWorldService
    {
        private readonly DbSet<User> _users;
        private ApplicationDbContext _dbContext;

        public RealWorldService(ApplicationDbContext dbContext)
        {
            _users = dbContext.Set<User>();
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = _users.ToList();
            return users;
        }

        public string AddUser(User user)
        {
            _users.Add(user);
            _dbContext.SaveChanges();
            return user.UserName;
        }
    }
}