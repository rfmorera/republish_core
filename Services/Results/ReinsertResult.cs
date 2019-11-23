using Models;
using Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Results
{
    public class ReinsertResult
    {
        public ReinsertResult(Anuncio anuncio)
        {
            Anuncio = anuncio;
            Success = true;
        }

        public ReinsertResult(Anuncio anuncio, Exception ex)
        {
            Anuncio = anuncio;
            Exception = ex;
            Success = false;
        }

        public Anuncio Anuncio { get; set; }
        public bool HasException
        {
            get
            {
                return Exception != null;
            }
        }
        public Exception Exception { get; set; }
        public bool Success { get; set; }
        public bool IsDeleted { get; set; }
    }
}
