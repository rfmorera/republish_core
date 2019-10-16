using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.AnuncioHelper
{
    class FormAnuncio
    {
        public FormAnuncio()
        {
            variables = new Variables();
        }
        public string operationName { get; } = "UpdateAdWithoutUser";
        public Variables variables { get; set; }
        public string query { get; } = "mutation UpdateAdWithoutUser($token: String!, $id: ID, $title: String, $description: String, $price: Float, $contactInfo: UpdateAdWithoutUserFormContactInfo!, $phone: String, $name: String, $subcategory: ID, $email: String, $captchaResponse: String!, $botScore: String, $images: [String]) {\n  updateAdWithoutUser(input: {token: $token, id: $id, title: $title, description: $description, price: $price, contactInfo: $contactInfo, phone: $phone, name: $name, subcategory: $subcategory, email: $email, captchaResponse: $captchaResponse, botScore: $botScore, images: $images}) {\n    ad {\n      id\n      title\n      description\n      price\n      phone\n      name\n      images {\n        edges {\n          node {\n            id\n            gcsKey\n            urls {\n              high\n              thumb\n              __typename\n            }\n            __typename\n          }\n          __typename\n        }\n        __typename\n      }\n      __typename\n    }\n    errors {\n      field\n      messages\n      __typename\n    }\n    clientMutationId\n    __typename\n  }\n}\n";
    }
}
