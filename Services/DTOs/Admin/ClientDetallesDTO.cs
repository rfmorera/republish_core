﻿using Microsoft.AspNetCore.Identity;
using Models;
using Services.DTOs.Registro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Admin
{
    public class ClientDetalles
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">IdentityUser del cliente en cuestion</param>
        /// <param name="Recargas">Lista de ultimas recargas recibidas</param>
        /// <param name="cuenta">Cuenta actual</param>
        /// <param name="GastoEsperadoActual">Gasto esperado en lo que resta de mes</param>
        /// <param name="GastoEsperadoProximo">Gasto esperado para el mes próximo</param>
        public ClientDetalles(IdentityUser client, 
                             IEnumerable<RecargaDetail> Recargas, 
                             Cuenta cuenta,
                             PrediccionIndicadores prediccionIndicadores, 
                             double GastoEsperadoProximo,
                             EstadisticaDiario Diario,
                             EstadisticaSemanal Semanal,
                             EstadisticaMensual Mensual)
        {
            Id = client.Id;
            UserName = client.UserName;
            Phone = client.PhoneNumber;
            this.Recargas = Recargas;
            Cuenta = new CuentaDTO(cuenta);
            this.PrediccionIndicadores = prediccionIndicadores;
            this.GastoEsperadoProximo = GastoEsperadoProximo;
            this.Diario = Diario;
            this.Semanal = Semanal;
            this.Mensual = Mensual;
        }

        public IEnumerable<RecargaDetail> Recargas { get; set; }
        public CuentaDTO Cuenta { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public PrediccionIndicadores PrediccionIndicadores { get; set; }
        public double GastoEsperadoProximo { get; set; }

        public EstadisticaDiario Diario { get; }
        public EstadisticaSemanal Semanal { get; }
        public EstadisticaMensual Mensual { get; }
    }
}
