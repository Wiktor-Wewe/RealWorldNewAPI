using Common;
using RealWorldNew.DAL.Entities;

using RealWorldNew.BAL;
using RealWorldNewAPI.Data;

namespace RealWorldNew.BAL
{
    public class RealWorldService : IRealWorldService
    {
        private ApplicationDbContext _dbContext;

        public RealWorldService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = _dbContext
                .Users
                .ToList();

            var dummy = new User
            {
                UserName = "wik"
            };
            users.Add(dummy);
            

            return (IEnumerable<User>)users;
        }
    }
}