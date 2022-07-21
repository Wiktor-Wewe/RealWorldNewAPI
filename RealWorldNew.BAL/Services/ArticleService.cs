using Microsoft.AspNetCore.Identity;
using RealWorldNew.Common;
using RealWorldNew.Common.Models;
using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.BAL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly UserManager<User> _userManager;

        public ArticleService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task AddArticle(string userId, ArticleUpload pack)
        {

        }
    }
}
