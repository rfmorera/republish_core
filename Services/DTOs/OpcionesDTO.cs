using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class OpcionesDTO
    {
        public OpcionesDTO(ClienteOpciones opt)
        {
            TemporizadoresUserEnable = opt.TemporizadoresUserEnable;
        }

        public bool TemporizadoresUserEnable { get; }
    }
}
