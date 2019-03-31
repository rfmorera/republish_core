using Models;
using Republish.Data;
using Republish.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Services.Impls
{
    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Repository<Grupo> _groupRepository;
        public GroupService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _groupRepository = new Repository<Grupo>(_dbContext);
        }

        public async Task Publish(string GroupId)
        {
            IEnumerable<Anuncio> anunciosList = await (from a in _dbContext.Set<Anuncio>()
                                                      where a.GroupId == GroupId
                                                      select a)
                                                      .ToListAsync();
        }
    }
}
