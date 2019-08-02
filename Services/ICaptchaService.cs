using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICaptchaService
    {
        Task<CaptchaKeys> Update2CaptchaKey(string key);
        Task<CaptchaKeys> GetCaptchaKeyAsync();
    }
}
