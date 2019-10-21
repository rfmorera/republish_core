using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class CaptchaAnswer
    {
        public CaptchaAnswer(string captchaId, string ans)
        {
            Id = captchaId;
            Answer = ans;
        }

        public string Id { get; set; }
        public string Answer { get; set; }
    }
}
