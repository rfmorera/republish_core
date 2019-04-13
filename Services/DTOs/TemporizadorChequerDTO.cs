using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class TemporizadorChequerDTO
    {
        public TemporizadorChequerDTO(Temporizador s)
        {
            GrupoId = s.GrupoId;
            Etapa = s.Etapa;
        }

        public string GrupoId { get; set; }
        public int Etapa { get; set; }
    }
}
