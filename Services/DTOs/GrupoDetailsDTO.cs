﻿using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class GrupoDetailsDTO
    {
        private Grupo grupo;

        public GrupoDetailsDTO(Grupo grupo, IEnumerable<AnuncioDTO> list)
        {
            this.grupo = grupo;
            Id = grupo.Id;
            Nombre = grupo.Nombre;
            Anuncios = list;
        }

        public string Id { get; set; }
        public string Nombre { get; set; }

        public IEnumerable<TemporizadorDTO> Temporizadores { get; set; }
        public IEnumerable<AnuncioDTO> Anuncios { get; set; }
    }
}
