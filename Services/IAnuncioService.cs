using Models;
using Services.DTOs.AnuncioHelper;
using Services.Results;
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

        Task UpdateTitle(string GrupoId);
        Task<IEnumerable<Anuncio>> GetByGroup(string GrupoId);
        Task DeleteAllByGroup(string GrupoId);

        Task NotifyDelete(List<string> list);

        Task<ReinsertResult> ReInsert(Anuncio anuncio, string Key2Captcha);
        Task<string> DeleteFromRevolico(FormDeleteAnuncio formDeleteAnuncio);
        FormUpdateAnuncio ParseFormAnuncio(string htmlAnuncio);
        Task Update(List<Anuncio> anunciosProcesados);
    }
}
