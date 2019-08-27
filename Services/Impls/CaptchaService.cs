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

        public async Task Add(CaptchaKeys captcha)
        {
            await repositoryCaptcha.AddAsync(captcha);
            await repositoryCaptcha.SaveChangesAsync();
        }

        public async Task Delete(string Id)
        {
            CaptchaKeys captcha = await repositoryCaptcha.FindAsync(c => c.Id == Id);
            
            repositoryCaptcha.Remove(captcha);

            await repositoryCaptcha.SaveChangesAsync();
        }

        public async Task<IEnumerable<CaptchaKeys>> GetCaptchaKeyAsync()
        {
            return (await repositoryCaptcha.GetAllAsync()).AsEnumerable();
        }

        public async Task<CaptchaKeys> Update2CaptchaKey(string Id, string Key)
        {
            CaptchaKeys captcha = await repositoryCaptcha.FindAsync(c => c.Id == Id);

            captcha.Key = Key;

            await repositoryCaptcha.UpdateAsync(captcha, captcha.Id);
            await repositoryCaptcha.SaveChangesAsync();

            return captcha;
        }
    }
}
