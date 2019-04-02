using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAnuncioService
    {
        Task AddAsync(IEnumerable<string> links);

        Task DeleteAsync(string Id);

        Task DeleteAllByGroup(string GrupoId);

        void Publish(string url);
    }
}
