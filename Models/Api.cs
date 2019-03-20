using System;
using System.Collections.Generic;

namespace Models
{
    public class Api
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceUrl { get; set; }
        public string ApiReturnUrl { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string ApyAccessToken { get; set; }
        public int? Userid { get; set; }
        public string RefreshToken { get; set; }
    }
}
