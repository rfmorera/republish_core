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
            Success = IsDeleted = NonRemoved = IsBaned = BadCaptcha = false;
            Success = true;
        }

        public ReinsertResult(Anuncio anuncio, Exception ex)
        {
            Anuncio = anuncio;
            Exception = ex;
            Success = IsDeleted = NonRemoved = IsBaned = BadCaptcha = false;
            if(ex.Message.Contains("Deteccion Anuncio Eliminado"))
            {
                IsDeleted = true;
            }
            else if (ex.Message.Contains("Non updated") || ex.Message.Contains("Baned CloudFlare"))
            {
                IsBaned = true;
            }
            else if (ex.Message.Contains("Error Removing from Revolico"))
            {
                NonRemoved = true;
            }
            else if (ex.Message.Contains("Captcha"))
            {
                BadCaptcha = true;
            }
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
        public bool NonRemoved { get; set; }
        public bool IsBaned { get; set; }
        public bool BadCaptcha { get; set; }
    }
}
