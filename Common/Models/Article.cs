using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.Models
{
    public class ArticleUpload
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<string> tagList { get; set; }
        public string body { get; set; }
    }

    public class articleAUP
    {
        public string slug { get; set; }
        public string titile { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public List<string> tagList { get; set; }
    }

    public class authorAUP
    {
        public string username { get; set; }
        public string bio { get; set; }
        public string image { get; set; }
        public bool following { get; set; }
    }

    public class ArticleUploadResponse
    {

    }
}
