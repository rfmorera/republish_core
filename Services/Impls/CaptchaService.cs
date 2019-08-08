using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Impls
{
    public class CaptchaService : ICaptchaService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<CaptchaKeys> repositoryCaptcha;

        public CaptchaService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            repositoryCaptcha = new Repository<CaptchaKeys>(dbContext);
        }

        public async Task<CaptchaKeys> GetCaptchaKeyAsync()
        {
            return (await repositoryCaptcha.GetAllAsync()).First();
        }

        public async Task<CaptchaKeys> Update2CaptchaKey(string key)
        {
            CaptchaKeys captcha = await GetCaptchaKeyAsync();
            repositoryCaptcha.Remove(captcha);

            captcha = new CaptchaKeys(key);

            await repositoryCaptcha.AddAsync(captcha);
            await repositoryCaptcha.SaveChangesAsync();

            return captcha;
        }
    }
}
