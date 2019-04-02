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

        Task DeleteAllByGroup(string GrupoId);

        void Publish(string url);
    }
}
