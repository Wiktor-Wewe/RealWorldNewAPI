using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.DAL.Entities
{
    internal class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ShortBio { get; set; }
        public string UrlProfile { get; set; }
        public string Password { get; set; }

        public virtual List<Article> Articles { get; set; }
        public virtual List<User> FollowedUsers { get; set; }
        public virtual List<Article> LikedArticles { get; set; }
    }
}
