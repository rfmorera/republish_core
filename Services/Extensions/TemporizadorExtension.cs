using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Extensions
{
    public static class TemporizadorExtension
    {
        public static bool IsValidDay(this Temporizador value)
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
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
    }
}
