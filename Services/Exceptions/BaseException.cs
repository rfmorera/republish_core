using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string Title)
        {
            this.Title = Title;
        }

        public BaseException(string Title, string message) : base(message)
        {
            this.Title = Title;
        }

        public string Title { get; set; }
        public string StackTraceIfNeeded
        {
            get
            {
                return String.Empty;
            }
        }
    }
}
