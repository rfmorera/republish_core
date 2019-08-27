using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICaptchaService
    {
        Task<CaptchaKeys> Update2CaptchaKey(string Id, string Key);
        Task<IEnumerable<CaptchaKeys>> GetCaptchaKeyAsync();
        Task Delete(string Id);
        Task Add(CaptchaKeys captcha);
    }
}
