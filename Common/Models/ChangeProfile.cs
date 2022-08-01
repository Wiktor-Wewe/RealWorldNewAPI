using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.DtoModels
{
    public class ChangeProfileContainer
    {
        [Required]
        public ChangeProfile user { get; set; }
    }

    public class ChangeProfile
    {
        public string? bio { get; set; }
        public string? email { get; set; }
        public string? image { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string username { get; set; }
    }
}
