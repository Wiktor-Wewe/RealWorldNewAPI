using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.Exceptions
{
    public class LikeException : Exception
    {
        public LikeException(string message) : base(message)
        {

        }
    }
}
