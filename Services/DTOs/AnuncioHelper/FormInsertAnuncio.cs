using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.AnuncioHelper
{
    public class FormInsertAnuncio
    {
        public FormInsertAnuncio(FormUpdateAnuncio formAnuncio)
        {
            variables = new VariablesInsert(formAnuncio.variables);
        }
        public string operationName { get; } = "CreateAdWithoutUser";
        public VariablesInsert variables { get; set; }
        public string query { get; } = "mutation CreateAdWithoutUser($title: String!, $description: String, $subcategory: ID!, $price: Float, $contactInfo: CreateAdWithoutUserFormContactInfo!, $phone: String, $name: String, $email: String!, $captchaResponse: String!, $botScore: String, $images: [String]) {\n  createAdWithoutUser(input: {title: $title, description: $description, subcategory: $subcategory, price: $price, contactInfo: $contactInfo, phone: $phone, email: $email, captchaResponse: $captchaResponse, botScore: $botScore, name: $name, images: $images}) {\n    ad {\n      id\n      price\n      imagesCount\n      contactInfo\n      subcategory {\n        id\n        title\n        slug\n        parentCategory {\n          id\n          title\n          slug\n          __typename\n        }\n        __typename\n      }\n      __typename\n    }\n    errors {\n      field\n      messages\n      __typename\n    }\n    token\n    clientMutationId\n    __typename\n  }\n}\n";
    }
}
