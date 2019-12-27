using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Services.DTOs
{
    public class GrupoDetailsDTO
    {
        private Grupo grupo;
        public GrupoDetailsDTO(Grupo grupo, IEnumerable<AnuncioDTO> list, IEnumerable<TemporizadorDTO> listT)
        {
            this.grupo = grupo;
            Id = grupo.Id;
            Nombre = grupo.Nombre;
            Anuncios = list;
            Temporizadores = listT;
            Costo = listT.Where(t => t.Enable && t.SystemEnable && t.UserEnable).Sum(t => t.Costo);
        }

        public string Id { get; set; }
        public string Nombre { get; set; }
        public double Costo { get; set; }

        public IEnumerable<TemporizadorDTO> Temporizadores { get; set; }
        public IEnumerable<AnuncioDTO> Anuncios { get; set; }
    }
}
