using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEmailRandomService
    {
        Task<IEnumerable<Emails>> GetList();
    }
}
