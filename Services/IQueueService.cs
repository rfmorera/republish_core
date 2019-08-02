using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IQueueService
    {
        Task AddMessageAsync(string captchaKey, IEnumerable<AnuncioDTO> list);
    }
}
