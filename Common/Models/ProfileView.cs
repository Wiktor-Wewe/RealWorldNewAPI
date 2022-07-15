using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.DtoModels
{
    public class ProfileViewContainer
    {
        public ProfileView profile { get; set; }
    }

    public class ProfileView
    {
        public string username { get; set; }
        public string bio { get; set; }
        public string image { get; set; }
        public bool following { get; set; }
    }
}
