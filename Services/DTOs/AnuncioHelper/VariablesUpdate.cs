using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.AnuncioHelper
{
    public class VariablesUpdate
    {
        public string token { get; set; }
        public string id { get; set; }
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

        [JsonIgnore]
        public string categoria { get; set; }
    }
}
