using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IQueueService
    {
        Task AddMessage(IEnumerable<AnuncioDTO> list);
    }
}
