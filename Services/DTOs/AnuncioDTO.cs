using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class AnuncioDTO
    {
        private Anuncio a;

        public AnuncioDTO(Anuncio a)
        {
            this.a = a;
        }
    }
}
