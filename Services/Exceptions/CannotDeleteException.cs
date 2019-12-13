using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class CannotDeleteException : Exception
    {
        public CannotDeleteException(string message) : base(message)
        {

        }
    }
}
