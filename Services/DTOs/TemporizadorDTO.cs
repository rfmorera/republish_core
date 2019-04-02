﻿using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.DTOs
{
    public class TemporizadorDTO
    {
        public TemporizadorDTO()
        {

        }

        public TemporizadorDTO(Temporizador t)
        {
            Nombre = t.Nombre;
            Lunes = t.Lunes;
            Martes = t.Martes;
            Miercoles = t.Miercoles;
            Jueves = t.Jueves;
            Viernes = t.Viernes;
            Sabado = t.Sabado;
            Domingo = t.Domingo;

            HoraInicio = t.HoraInicio;
            HoraFin = t.HoraFin;

            IntervaloHoras = t.IntervaloHoras;
            IntervaloMinutos = t.IntervaloMinutos;

            Etapa = t.Etapa;
        }
        public string Id { get; set; }

        [Display(Name = "Nombre", Order = 0, ShortName = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Lunes", Order = 1, ShortName = "L")]
        public bool Lunes { get; set; }
        [Display(Name = "Martes", Order = 2, ShortName = "Ma")]
        public bool Martes { get; set; }
        [Display(Name = "Miercoles", Order = 3, ShortName = "Mi")]
        public bool Miercoles { get; set; }
        [Display(Name = "Jueves", Order = 4, ShortName = "J")]
        public bool Jueves { get; set; }
        [Display(Name = "Viernes", Order = 5, ShortName = "V")]
        public bool Viernes { get; set; }
        [Display(Name = "Sábado", Order = 6, ShortName = "S")]
        public bool Sabado { get; set; }
        [Display(Name = "Domingo", Order = 7, ShortName = "D")]
        public bool Domingo { get; set; }


        [Display(Name = "Hora de Inicio", Order = 8, ShortName = "HI")]
        public DateTime HoraInicio { get; set; }
        [Display(Name = "Hora de Fin", Order = 9, ShortName = "HF")]
        public DateTime HoraFin { get; set; }

        [Display(Name = "Intervalo en horas", Order = 10, ShortName = "IH")]
        public int IntervaloHoras { get; set; }
        [Display(Name = "Intervalo en minutos", Order = 11, ShortName = "IM")]
        public int IntervaloMinutos { get; set; }

        [Display(Name = "Cantidad de anuncios por intervalo", Order = 11)]
        [Range(0,300)]
        public int Etapa { get; set; }
    }
}
