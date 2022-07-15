﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.DtoModels
{
    public class RegisterUserPack
    {
        public RegisterUserDto user { get; set; }
    }

    public class RegisterUserDto
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
