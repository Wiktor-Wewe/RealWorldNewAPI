using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNew.Common.Exceptions
{
    public class CommentException : Exception
    {
        public CommentException(string message) : base(message)
        {

        }
    }
}
