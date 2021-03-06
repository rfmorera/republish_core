﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Services.DTOs.Registro;
using System.Linq;
using Republish.Extensions;

namespace Services.Impls
{
    public class EstadisticasService : IEstadisticasService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Registro> repositoryRegistro;

        public EstadisticasService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            repositoryRegistro = new Repository<Registro>(dbContext);
        }
        
        public async Task<EstadisticaDiario> GetDiario(string clientId)
        {
            DateTime UtcCuba = DateTime.Now.ToUtcCuba();
            
            return await GetDiarioDetail(clientId, UtcCuba);
        }

        private async Task<EstadisticaDiario> GetDiarioDetail(string clientId, DateTime UtcCuba)
        {
            IEnumerable<Registro> registros = await repositoryRegistro.FindAllAsync(r => 
                                                                                    (r.DateCreated.DayOfYear == UtcCuba.DayOfYear 
                                                                                    && r.DateCreated.Year == UtcCuba.Year
                                                                                    && r.UserId == clientId));
            EstadisticaDiario dia = new EstadisticaDiario(registros, UtcCuba);
            return dia;
        }

        public async Task<EstadisticaMensual> GetMensual(string clientId)
        {
            DateTime UtcCuba = DateTime.Now.ToUtcCuba();
            List<EstadisticaDiario> days = new List<EstadisticaDiario>();
            while(UtcCuba.Day > 1)
            {
                days.Add(await GetDiarioDetail(clientId, UtcCuba));
                UtcCuba = UtcCuba.AddDays(-1);
            }
            days.Add(await GetDiarioDetail(clientId, UtcCuba));

            int tot = days.Sum(r => r.Total);
            double gasto = Math.Round(days.Sum(r => r.Gasto), 3);
            EstadisticaMensual mes = new EstadisticaMensual(days, tot, gasto);
            return mes;
        }

        public async Task<EstadisticaSemanal> GetSemanal(string clientId)
        {
            DateTime UtcCuba = DateTime.Now.ToUtcCuba();
            List<EstadisticaDiario> last7Days = new List<EstadisticaDiario>(7);
            for(int i = 6; i >= 0; i--)
            {
                last7Days.Add(await GetDiarioDetail(clientId, UtcCuba.AddDays(-i)));
            }

            int total = last7Days.Sum(d => d.Total);
            double gasto = Math.Round(last7Days.Sum(d => d.Gasto), 3);
            EstadisticaSemanal semana = new EstadisticaSemanal(last7Days, total, UtcCuba, gasto);

            return semana;
        }
    }
}
