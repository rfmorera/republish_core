using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class AnuncioEliminadoException : Exception
    {
        public AnuncioEliminadoException(string message, string uri) : base(message)
        {
            Uri = uri;
        }

        public string Uri { get; set; }

    }
}
