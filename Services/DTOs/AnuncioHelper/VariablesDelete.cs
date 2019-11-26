using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.AnuncioHelper
{
    public class VariablesDelete
    {
        public VariablesDelete(VariablesUpdate variables)
        {
            token = variables.token;
            id = variables.id;
        }

        public string token { get; set; }
        public string id { get; set; }
    }
}
