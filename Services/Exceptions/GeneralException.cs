using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class GeneralException : Exception
    {
        public string uri { get; set; }
        public GeneralException(string message, string _uri) : base(message)
        {
            uri = _uri;
        }
    }
}
