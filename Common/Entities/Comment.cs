using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.DAL.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Text { get; set; }
    }
}
