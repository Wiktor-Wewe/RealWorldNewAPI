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
        public string Title { get; set; }
        public string Topic { get; set; }
        public string Text { get; set; }

        public virtual User Author { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual List<Comment> Comments { get; set; }
    }
}
