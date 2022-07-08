using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RealWorldNew.DAL.Entities
{
    public class User : IdentityUser<int>
    {
        public string ShortBio { get; set; }
        public string UrlProfile { get; set; }

        public virtual List<User> FollowedUsers { get; set; }
        public virtual List<Article> Articles { get; set; }
        public virtual List<Article> LikedArticles { get; set; }
        public virtual List<Article> FollowedArticles { get; set; }
    }
}
