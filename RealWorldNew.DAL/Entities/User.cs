using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RealWorldNew.DAL.Entities
{
    public class User : IdentityUser
    {
        public string? ShortBio { get; set; }
        public string? UrlProfile { get; set; }

        public List<User> FollowedUsers { get; set; } = new List<User>();
        public List<Article> Articles { get; set; } = new List<Article>();
        public List<Article> LikedArticles { get; set; } = new List<Article>();
        public List<Article> FollowedArticles { get; set; } = new List<Article>();
    }
}
