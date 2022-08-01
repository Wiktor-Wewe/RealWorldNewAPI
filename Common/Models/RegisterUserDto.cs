using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.DtoModels
{
    public class RegisterUserPack
    {
        [Required]
        public RegisterUserDto user { get; set; }
    }

    public class RegisterUserDto
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
