using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.DAL.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Slug => $"{Title}-{Id}";
        public string Title { get; set; }
        public string Topic { get; set; }
        public string Text { get; set; }
        public int NumberOfLikes { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public User Author { get; set; } = new User();
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
