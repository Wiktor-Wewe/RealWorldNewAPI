using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.DtoModels
{
    public class LoginUserPack
    {
        public LoginUser user { get; set; }
    }

    public class ErrorLoginPack
    {
        public ErrorLoginPack errors { get; set; }
    }

    public class LoginUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ErrorLoginUser
    {
        public List<string> email { get; set; }
        public List<string> password { get; set; }
    }
}
