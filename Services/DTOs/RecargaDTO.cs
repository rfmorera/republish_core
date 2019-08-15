using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class RecargaDTO
    {
        public RecargaDTO(string Operador, string Client, int Monto)
        {
            OperardorId = Operador;
            ClientId = Client;
            this.Monto = Monto;
        }

        public int Monto { get; set; }

        public string ClientId { get; set; }

        public string OperardorId { get; set; }

        public Recarga ToRecarga(DateTime now)
        {
            Recarga r = new Recarga();
            r.Monto = Monto;
            r.ClientId = ClientId;
            r.OperardorId = OperardorId;
            r.DateCreated = now;
            return r;
        }
    }
}
