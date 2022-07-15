using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.DtoModels
{
    public class ViewUserDto
    {
        public string UserName { get; set; }
        public string ShortBio { get; set; }
        public string UrlProfile { get; set; }
        public virtual List<Article> Articles { get; set; }
        public virtual List<Article> LikedArticles { get; set; }
    }
}
