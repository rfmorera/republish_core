using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class CannotDeleteRecord : Exception
    {
        public CannotDeleteRecord(string message) : base(message)
        {

        }
    }
}
