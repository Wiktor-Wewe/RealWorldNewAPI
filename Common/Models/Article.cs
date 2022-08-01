using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.Models
{
    public class ArticleUploadPack
    {
        [Required]
        public ArticleUpload article { get; set; }
    }

    public class ArticleUpload
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public List<string> tagList { get; set; } = new List<string>();
        [Required]
        public string body { get; set; }
    }

    public class articleAUP
    {
        public string slug { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string body { get; set; }
        [Required]
        public List<string> tagList { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public bool favorited { get; set; }
        public int favoritesCount { get; set; }
        public authorAUP author { get; set; }
    }

    public class authorAUP
    {
        public string username { get; set; }
        public string? bio { get; set; }
        public string? image { get; set; }
        public bool following { get; set; }
    }

    public class ArticleUploadResponse
    {
        [Required]
        public articleAUP article { get; set; }
    }

    public class MultiArticleResponse
    {
        public List<articleAUP> articles { get; set; }
        public int articlesCount { get; set; }
    }
}
