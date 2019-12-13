using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs
{
    public class CaptchaAnswer
    {
        public CaptchaAnswer(string access_token, string captchaIdv2, string captchaIdv3, string ansv2, string ansv3)
        {
            Idv2 = captchaIdv2;
            Idv3 = captchaIdv3;
            Answerv2 = ansv2;
            Answerv3 = ansv3;
            AccessToken = access_token;
        }

        public string Idv2 { get; set; }
        public string Idv3 { get; set; }
        public string AccessToken { get; set; }
        public string Answerv2 { get; set; }
        public string Answerv3 { get; set; }
    }
}
