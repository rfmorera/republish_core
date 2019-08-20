using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToExceptionString(this Exception ex)
        {
            return ex.Source + "--\n--" + ex.StackTrace + "--\n--" + ex.Message;
        }
    }
}
