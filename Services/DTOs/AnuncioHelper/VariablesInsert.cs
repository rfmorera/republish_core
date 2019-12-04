using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.AnuncioHelper
{
    public class VariablesInsert
    {
        public VariablesInsert()
        {
            subcategory = email = title = contactInfo = captchaResponse = string.Empty;
            description = name = phone = string.Empty;
        }

        public VariablesInsert(VariablesUpdate variables)
        {
            subcategory = variables.subcategory;
            email = variables.email;
            title = variables.title;
            contactInfo = variables.contactInfo;
            captchaResponse = variables.captchaResponse;
            botScore = variables.botScore;
            images = variables.images;
            price = variables.price;
            description = variables.description;
            name = variables.name;
            phone = variables.phone;
        }

        public string subcategory { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string contactInfo { get; set; }
        public string captchaResponse { get; set; }
        public string botScore { get; set; }
        public string[] images { get; set; }
        public int price { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }
}
