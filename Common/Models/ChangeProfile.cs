using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.DtoModels
{
    public class ChangeProfileContainer
    {
        public ChangeProfile user { get; set; }
    }

    public class ChangeProfile
    {
        public string bio { get; set; }
        public string email { get; set; }
        public string image { get; set; }
        public string password { get; set; }
        public string username { get; set; }
    }
}
