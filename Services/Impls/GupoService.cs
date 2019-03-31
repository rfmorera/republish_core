using Models;
using Republish.Data;
using Republish.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;

namespace Services.Impls
{
    public class GupoService : IGrupoService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Repository<Grupo> _groupRepository;
        public GupoService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _groupRepository = new Repository<Grupo>(_dbContext);
        }

        public Task Add(GrupoDTO grupoDTO)
        {
            throw new NotImplementedException();
        }

        public Task Publish(string Id)
        {
            throw new NotImplementedException();
        }

        public Task Remove(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
