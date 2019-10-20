using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class BadCaptchaException : Exception
    {
        public string uri { get; set; }
        public BadCaptchaException(string message, string _uri) : base(message)
        {
            uri = _uri;
        }
    }
}
