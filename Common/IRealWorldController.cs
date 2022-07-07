using RealWorldNew.DAL.Entities;

namespace Common
{
    public interface IRealWorldController
    {
        IEnumerable<User> GetAllUsers();
    }
}