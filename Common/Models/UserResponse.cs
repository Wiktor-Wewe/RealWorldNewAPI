using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.DtoModels
{
    public class UserResponseContainer
    {
        public UserResponse user { get; set; }
    }

    public class UserResponse
    {
        public string email { get; set; }
        public string username { get; set; }
        public string bio { get; set; }
        public string image { get; set; }
        public string token { get; set; }
    }
}
