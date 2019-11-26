using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class AnuncioEliminadoException : BaseException
    {
        public AnuncioEliminadoException(string message, string uri) : base(message)
        {
            Uri = uri;
            Title = "Anuncio Eliminado";
        }

        public string Uri { get; set; }

    }
}
