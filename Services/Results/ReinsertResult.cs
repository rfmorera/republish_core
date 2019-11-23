using Models;
using Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Results
{
    public class ReinsertResult
    {
        public Anuncio Anuncio { get; set; }
        public bool HasException { get; set; }
        public BaseException Exception { get; set; }
        public bool Success { get; set; }
        public bool IsDeleted { get; set; }
    }
}
