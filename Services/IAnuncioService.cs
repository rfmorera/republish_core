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

        Task UpdateTitle(string GrupoId);
        Task<IEnumerable<Anuncio>> GetByGroup(string GrupoId);
        Task DeleteAllByGroup(string GrupoId);
        Task<IEnumerable<Anuncio>> GetAnunciosToUpdate(string GroupId, int Etapa);

        Task NotifyDelete(List<Anuncio> list);

        Task<ReinsertResult> ReInsert(Anuncio anuncio, string Key2Captcha, string email);
        Task<bool> DeleteFromRevolico(FormDeleteAnuncio formDeleteAnuncio);
        Task<bool> DeleteFromRevolico(string url);
        Task Update(List<Anuncio> anunciosProcesados);

        FormUpdateAnuncio ParseFormAnuncio(string htmlAnuncio);
        Task<FormInsertAnuncio> Retrieve(string url);
    }
}
