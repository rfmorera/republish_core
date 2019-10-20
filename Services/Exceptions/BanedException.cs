using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class BanedException : Exception
    {
        public string uri { get; set; }
        public BanedException(string message, string _uri) : base(message)
        {
            uri = _uri;
        }
    }
}
