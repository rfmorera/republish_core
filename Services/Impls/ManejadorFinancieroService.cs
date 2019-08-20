using BlueDot.Data.UnitsOfWorkInterfaces;
using Models;
using Republish.Extensions;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Services.Impls
{
    public class ManejadorFinancieroService : IManejadorFinancieroService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManejadorFinancieroService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RecargarUsuario(RecargaDTO recargaDTO)
        {
            DateTime now = DateTime.Now.ToUtcCuba();
            Recarga r = recargaDTO.ToRecarga(now);
            await _unitOfWork.Recarga.AddAsync(r);

            Cuenta c = await _unitOfWork.Cuenta.FindAsync(t => t.UserId == r.ClientId);
            c.Saldo += r.Monto;

            await _unitOfWork.Cuenta.UpdateAsync(c, c.Id);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<double> GetSaldo(string UserId)
        {
            return (await _unitOfWork.Cuenta.FindAsync(t => t.UserId == UserId)).Saldo;
        }

        public async Task InicializarUsuario(string UserId)
        {
            Cuenta c = new Cuenta
            {
                UserId = UserId,
                Saldo = 0
            };

            await _unitOfWork.Cuenta.AddAsync(c);
        }

        public async Task<IEnumerable<string>> FacturarRegistros()
        {
            DateTime UtcCuba = DateTime.Now.ToUtcCuba();
            IEnumerable<Registro> lst = (await _unitOfWork.Registro.FindAllAsync(t => t.Facturado == false)).OrderBy(r => r.UserId);

            Cuenta ct = null;
            List<string> CeroBalanceUsers = new List<string>();
            bool flag = false;
            
            foreach(Registro rg in lst)
            {
                if(ct is null || ct.UserId != rg.UserId)
                {
                    flag = false;
                    ct = await _unitOfWork.Cuenta.FindAsync(c => c.UserId == rg.UserId);
                }

                ct.Saldo -= rg.Gasto;
                rg.Facturado = true;
                ct.LastUpdate = UtcCuba;

                if(!flag && ct.Saldo < 0)
                {
                    CeroBalanceUsers.Add(ct.UserId);
                    flag = true;
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return CeroBalanceUsers;
        }

        public async Task<Cuenta> GetCuenta(string UserId)
        {
            return (await _unitOfWork.Cuenta.FindAsync(t => t.UserId == UserId));
        }

        public async Task<bool> HasBalance(string UserId)
        {
            Cuenta ct = await GetCuenta(UserId);
            return ct.Saldo > 0;
        }
    }
}
