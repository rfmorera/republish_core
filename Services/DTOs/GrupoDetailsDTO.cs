using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class GrupoDetailsDTO
    {
        private Grupo grupo;
        private IEnumerable<AnuncioDTO> list;

        public GrupoDetailsDTO(Grupo grupo, IEnumerable<AnuncioDTO> list)
        {
            this.grupo = grupo;
            this.list = list;
        }
    }
}
