using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAnuncioService
    {
        Task AddAsync(string GrupoId, string[] links);

        Task DeleteAsync(string Id);
        Task DeleteAsync(List<string> list);
        Task DeleteAsync(IEnumerable<Anuncio> anuncios);

        Task DeleteAllByGroup(string GrupoId);

        Task NotifyDelete(List<string> list);

        Task Publish(string url, string Key2Captcha);
    }
}
