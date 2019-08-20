﻿using Models;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITemporizadorService
    {
        Task AddAsync(Temporizador temporizador, bool SystemEnable);

        Task DeleteAsync(string Id);

        Task<IEnumerable<Temporizador>> GetByGroup(string GroupId);

        Task DeleteAllByGroup(string GrupoId);

        Task SetEnable(string UserId, bool SystemEnable);
    }
}
