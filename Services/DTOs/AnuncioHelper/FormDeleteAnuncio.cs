using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.AnuncioHelper
{
    public class FormDeleteAnuncio
    {
        public FormDeleteAnuncio(FormUpdateAnuncio formAnuncio)
        {
            variables = new VariablesDelete(formAnuncio.variables);
        }
        public string operationName { get; } = "DeleteAdWithoutUser";
        public VariablesDelete variables { get; set; }
        public string query { get; } = "mutation DeleteAdWithoutUser($token: String!, $id: ID) {\n  deleteAdWithoutUser(input: {token: $token, id: $id}) {\n    ad {\n      id\n      __typename\n    }\n    errors {\n      field\n      messages\n      __typename\n    }\n    clientMutationId\n    __typename\n  }\n}\n";
    }
}
