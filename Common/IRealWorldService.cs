﻿using RealWorldNew.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IRealWorldService
    {
        IEnumerable<User> GetAllUsers();
        string AddUser(User user);
    }
}
