using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IChequerService
    {
        Task CheckAllTemporizadores();
        Task ResetAll();
    }
}
