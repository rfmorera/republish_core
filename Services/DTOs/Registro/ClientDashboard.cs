﻿using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Registro
{
    public class ClientDashboard
    {
        public ClientDashboard(Cuenta ct, EstadisticaDiario dia, EstadisticaSemanal semana, EstadisticaMensual mensual, ClienteOpciones opt, PrediccionIndicadores prediccion)
        {
            Cnt = new CuentaDTO(ct);
            Opciones = new OpcionesDTO(opt);
            Diario = dia;
            Semanal = semana;
            Mensual = mensual;
            Prediccion = prediccion;
        }
        
        public CuentaDTO Cnt { get; }
        public EstadisticaDiario Diario { get; }
        public EstadisticaSemanal Semanal { get; }
        public EstadisticaMensual Mensual { get; }
        public OpcionesDTO Opciones { get; }
        public PrediccionIndicadores Prediccion { get; }
    }
}
