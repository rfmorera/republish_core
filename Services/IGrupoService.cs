using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IGrupoService
    {
        Task Add(GrupoDTO grupoDTO);

        Task Remove(string Id);

        Task Publish(string Id);
    }
}
