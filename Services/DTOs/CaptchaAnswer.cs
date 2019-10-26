using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class CaptchaAnswer
    {
        public CaptchaAnswer(string access_token, string captchaId, string ans)
        {
            Id = captchaId;
            Answer = ans;
            AccessToken = access_token;
        }

        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string Answer { get; set; }
    }
}
