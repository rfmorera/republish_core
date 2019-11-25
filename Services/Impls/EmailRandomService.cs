using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Services.Impls
{
    public class EmailRandomService : IEmailRandomService
    {
        private readonly IRepository<Emails> _emailsRepository;

        public EmailRandomService(ApplicationDbContext context)
        {
            _emailsRepository = new Repository<Emails>(context);
        }

        public async Task<IEnumerable<Emails>> GetList()
        {
            return (await _emailsRepository.GetAllAsync()).ToList();
        }

        public async Task Add(Emails emails)
        {
            await _emailsRepository.AddAsync(emails);
            await _emailsRepository.SaveChangesAsync();
        }

        public async Task Delete(string Id)
        {
            Emails email = await _emailsRepository.FindAsync(e => e.Id == Id);
            _emailsRepository.Remove(email);
            await _emailsRepository.SaveChangesAsync();
        }
    }
}
