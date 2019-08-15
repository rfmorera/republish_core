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
    }
}
