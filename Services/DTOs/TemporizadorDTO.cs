using Microsoft.AspNetCore.Identity;
using Models;
using Republish.Extensions;
using Services.Extensions;
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
            Id = t.Id;
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
            Next = t.NextExecution;

            IntervaloHoras = t.IntervaloHoras;
            IntervaloMinutos = t.IntervaloMinutos;

            Etapa = t.Etapa;

            GrupoId = t.GrupoId;
            UserId = t.UserId;

            Enable = t.Enable;
            UserEnable = t.UserEnable;
            SystemEnable = t.SystemEnable;
            Ejecutandose = t.IsValidDay();
            TimeSpan now = DateTime.Now.ToUtcCuba().TimeOfDay;
            if (HoraInicio <= now && now <= HoraFin && Ejecutandose)
            {
                Ejecutandose = true;
            }
            else
            {
                Ejecutandose = false;
            }
        }

        public Temporizador BuildModel(IdentityUser user)
        {
            if (!Lunes && !Martes && !Miercoles && !Jueves && !Viernes && !Sabado && !Domingo)
            {
                Lunes = Martes = Miercoles = Jueves = Viernes = Sabado = Domingo = true;
            }
            Temporizador t = new Temporizador();
            t.Id = Id;
            t.Nombre = Nombre;
            t.Lunes = Lunes;
            t.Martes = Martes;
            t.Miercoles = Miercoles;
            t.Jueves = Jueves;
            t.Viernes = Viernes;
            t.Sabado = Sabado;
            t.Domingo = Domingo;

            t.HoraInicio = HoraInicio;
            t.HoraFin = HoraFin;
            t.NextExecution = HoraInicio;

            t.IntervaloHoras = IntervaloHoras;
            t.IntervaloMinutos = IntervaloMinutos;

            t.Etapa = Etapa;

            t.GrupoId = GrupoId;
            t.UserId = user.Id;
            t.Enable = true;
            return t;
        }

        public override string ToString()
        {
            string tmp = "<br>";
            tmp += "Días Habilitados: ";
            if(Lunes && Martes && Miercoles && Jueves && Viernes && Sabado && Domingo)
            {
                tmp += "Todos";
            }
            else
            {
                if (Lunes)
                {
                    tmp += "Lunes, ";
                }
                if (Martes)
                {
                    tmp += "Martes, ";
                }
                if (Miercoles)
                {
                    tmp += "Miercoles, ";
                }
                if (Jueves)
                {
                    tmp += "Jueves, ";
                }
                if (Viernes)
                {
                    tmp += "Viernes, ";
                }
                if (Sabado)
                {
                    tmp += "Sábado, ";
                }
                if (Domingo)
                {
                    tmp += "Domingo, ";
                }
                tmp = tmp.Substring(0, tmp.Length - 2);
            }

            tmp += string.Format("<br>Hora Inicio {0} <br> Hora Fin {1} <br> Próxima ejecución {2}", HoraInicio.ToString(), HoraFin.ToString(), Next.ToString());
            tmp += string.Format("<br>Intervalo Horas: {0} Minutos: {1}", IntervaloHoras, IntervaloMinutos);
            if(Etapa == 0)
            {
                tmp += "<br>Todos los anuncios por ejecución";
            }
            else
            {
                tmp += string.Format("<br>{0} anuncios por ejecución", Etapa);
            }
            return tmp;
        }
        public string Id { get; set; }

        public bool Enable { get; set; }
        public bool UserEnable { get; set; }
        public bool SystemEnable { get; set; }
        public bool Ejecutandose { get; set; }

        public string getEstadoClass
        {
            get
            {
                if (!SystemEnable)
                {
                    return "label-danger";
                }
                else if(!UserEnable)
                {
                    return "label-warning";
                }
                else if (!Enable)
                {
                    return "";
                }
                else
                {
                    if(Ejecutandose)
                    {
                        return "label-primary";
                    }
                    else
                    {
                        return "label-inverse";
                    }
                }
            }
        }

        public string getEstado
        {
            get
            {
                if (!SystemEnable)
                {
                    return "Saldo Insuficiente";
                }
                else if (!UserEnable)
                {
                    return "Temporizadores Apagados";
                }
                else if (!Enable)
                {
                    return "Deshabilidato";
                }
                else
                {
                    if(Ejecutandose)
                    {
                        return "Activo";
                    }
                    else
                    {
                        return "Esperando";
                    }
                }
            }
        }

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
        public TimeSpan HoraInicio { get; set; }
        [Display(Name = "Hora de Fin", Order = 9, ShortName = "HF")]
        public TimeSpan HoraFin { get; set; }
        public TimeSpan Next { get; set; }

        [Display(Name = "Intervalo en horas", Order = 10, ShortName = "IH")]
        public int IntervaloHoras { get; set; }
        [Display(Name = "Intervalo en minutos", Order = 11, ShortName = "IM")]
        public int IntervaloMinutos { get; set; }

        [Display(Name = "Cantidad de anuncios por intervalo", Order = 11)]
        [Range(0,300)]
        public int Etapa { get; set; }

        public string GrupoId { get; set; }
        public string UserId { get; set; }
    }
}
