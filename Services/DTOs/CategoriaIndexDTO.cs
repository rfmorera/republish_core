using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Services.DTOs
{
    public class CategoriaIndexDTO
    {
        private Categoria c;

        public CategoriaIndexDTO()
        {

        }

        public CategoriaIndexDTO(Categoria c)
        {
            Id = c.Id;
            Nombre = c.Nombre;
            Descripcion = c.Descripcion;
            Activo = c.Activo;
            Grupos = c.Grupos != null ? c.Grupos.Count : 0;
        }

        internal Categoria BuildModel(string UserId)
        {
            Categoria categoria = new Categoria();
            categoria.Nombre = Nombre;
            categoria.Descripcion = Descripcion;
            categoria.Activo = true;
            categoria.UserId = UserId;

            return categoria;
        }

        public string Id { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        public int Grupos { get; set; }
    }
}
