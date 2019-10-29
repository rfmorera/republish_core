using Models;
using Republish.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Extensions
{
    public static class TemporizadorExtension
    {
        public static bool IsValidDay(this Temporizador value, DateTime utcCuba)
        {
            DayOfWeek today = utcCuba.DayOfWeek;
            switch (today)
            {
                case DayOfWeek.Sunday:
                    return value.Domingo;
                case DayOfWeek.Monday:
                    return value.Lunes;
                case DayOfWeek.Tuesday:
                    return value.Martes;
                case DayOfWeek.Wednesday:
                    return value.Miercoles;
                case DayOfWeek.Thursday:
                    return value.Jueves;
                case DayOfWeek.Friday:
                    return value.Viernes;
                case DayOfWeek.Saturday:
                    return value.Sabado;
            }
            throw new Exception("Invalid day of week");
        }

        public static double Costo(this Temporizador value, double costoAnuncio, int cantidadAnuncios, DateTime utcCuba)
        {
            double val = 0;
            int etapa = value.Etapa == 0 ? cantidadAnuncios : value.Etapa;
            int intervaloMinutosTotal = value.IntervaloHoras * 60 + value.IntervaloMinutos;
            TimeSpan time = value.HoraFin - value.HoraInicio;
            int minutosEjecucion = (int) time.TotalMinutes;
            int cantidadEjecuciones = minutosEjecucion / intervaloMinutosTotal;
            double costoDiario = costoAnuncio * cantidadEjecuciones * etapa;

            DateTime tmp = utcCuba;

            for (int i = 0; i < 31; i++)
            {
                if (value.IsValidDay(tmp))
                {
                    val += costoDiario;
                }
                tmp = tmp.AddDays(1);
                if(tmp.Month != utcCuba.Month)
                {
                    return val;
                }
            }

            throw new Exception("Temporizador Costo> Month Never Change");
        }
    }
}
