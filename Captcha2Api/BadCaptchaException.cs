using System;
using System.Collections.Generic;
using System.Text;

namespace Captcha2Api.Exceptions
{
    public class BadCaptchaException : Exception
    {
        public BadCaptchaException(string message) : base(message)
        {
        }
    }
}
