using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Services.Impls
{
    public class ValidationService : IValidationService
    {
        private readonly IRepository<Anuncio> _anuncioRepository;
        private readonly INotificationsService _notificationsService;

        public ValidationService(ApplicationDbContext context, INotificationsService notificationsService)
        {
            _anuncioRepository = new Repository<Anuncio>(context);
            _notificationsService = notificationsService;
        }

        public async Task VerifyPublication(ICollection<string> link)
        {
            link = link.Distinct().ToList();
            IEnumerable<Anuncio> list = await _anuncioRepository.FindAllAsync(a => link.Contains(a.Url));
            await Task.Delay(TimeSpan.FromSeconds(40));
        }
    }
}
