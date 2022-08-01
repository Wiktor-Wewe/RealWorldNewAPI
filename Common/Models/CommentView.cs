using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.Models
{
    public class CommentPack
    {
        public CommentView? Comment { get; set; }
        public List<CommentView>? Comments { get; set; }
    }

    public class CommentView
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public string Body { get; set; }
        public authorAUP? Author { get; set; }
    }
}
