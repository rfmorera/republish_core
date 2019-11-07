using BlueDot.Data.UnitsOfWorkInterfaces;
using Models;
using Republish.Extensions;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services.Extensions;

namespace Services.Impls
{
    public class ManejadorFinancieroService : IManejadorFinancieroService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationsService _notificationsService;

        public ManejadorFinancieroService(IUnitOfWork unitOfWork, INotificationsService notificationsService)
        {
            _unitOfWork = unitOfWork;
            _notificationsService = notificationsService;
        }

        public async Task RecargarUsuario(RecargaDTO recargaDTO)
        {
            DateTime now = DateTime.Now.ToUtcCuba();
            Recarga r = recargaDTO.ToRecarga(now);
            await _unitOfWork.Recarga.AddAsync(r);

            Cuenta c = await _unitOfWork.Cuenta.FindAsync(t => t.UserId == r.ClientId);
            c.Saldo += r.Monto;
            Notificacion notificacion = new Notificacion()
            {
                UserId = recargaDTO.ClientId,
                DateCreated = now,
                Mensaje = String.Format("Cuenta recargada: ${0}. Su saldo actual es de {1}", recargaDTO.Monto, c.Saldo),
                Readed = false
            };

            await _notificationsService.Add(notificacion);

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
                Saldo = 0,
                CostoAnuncio = 0.006
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

            foreach (Registro rg in lst)
            {
                if (ct is null || ct.UserId != rg.UserId)
                {
                    flag = false;
                    ct = await _unitOfWork.Cuenta.FindAsync(c => c.UserId == rg.UserId);
                }

                ct.Saldo -= rg.Gasto;
                rg.Facturado = true;
                ct.LastUpdate = UtcCuba;

                if (!flag && ct.Saldo < 0)
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

        public async Task<Cuenta> GetCuentaIncludeAll(string UserId)
        {
            return await _unitOfWork.Cuenta.QueryAll()
                                            .Where(t => t.UserId == UserId)
                                            .Include(t => t.User)
                                            .Select(s => s)
                                            .SingleAsync();
        }

        public async Task<bool> HasBalance(string UserId)
        {
            Cuenta ct = await GetCuenta(UserId);
            return ct.Saldo > 0;
        }

        public async Task<double> CostoAnuncio(string UserId)
        {
            return (await GetCuenta(UserId)).CostoAnuncio;
        }

        public async Task<double> SetCostoAnuncio(string UserId, double CostoAnuncio)
        {
            Cuenta cnt = await GetCuenta(UserId);
            cnt.CostoAnuncio = CostoAnuncio;
            await UpdateCuenta(cnt);
            return CostoAnuncio;
        }

        public async Task<IEnumerable<RecargaDetail>> GetRecargasByAgente(string agentId)
        {
            return await _unitOfWork.Recarga.QueryAll().Where(r => r.OperardorId == agentId)
                                                                    .Include(o => o.Client)
                                                                    .Include(a => a.Operardor)
                                                                    .OrderByDescending(t => t.DateCreated)
                                                                    .Take(50)
                                                                    .Select(e => new RecargaDetail(e.Client.UserName, e.Operardor.UserName, e.Monto, e.DateCreated))
                                                                    .ToListAsync();
        }

        public async Task<IEnumerable<RecargaDetail>> GetRecargasByClient(string clientId)
        {
            return await _unitOfWork.Recarga.QueryAll().Where(r => r.ClientId == clientId)
                                                                    .Include(o => o.Client)
                                                                    .Include(a => a.Operardor)
                                                                    .OrderByDescending(t => t.DateCreated)
                                                                    .Take(50)
                                                                    .Select(e => new RecargaDetail(e.Client.UserName, e.Operardor.UserName, e.Monto, e.DateCreated))
                                                                    .ToListAsync();
        }

        public async Task<double> GetMontoMesByAgente(string agentId, DateTime date)
        {
            return (await _unitOfWork.Recarga.FindAllAsync(r => r.OperardorId == agentId
                                                           && r.DateCreated.Month == date.Month
                                                           && r.DateCreated.Year == date.Year))
                                            .Sum(t => t.Monto);
        }

        public async Task<Cuenta> UpdateCuenta(Cuenta cuenta)
        {
            await _unitOfWork.Cuenta.UpdateAsync(cuenta, cuenta.Id);
            await _unitOfWork.SaveChangesAsync();
            return cuenta;
        }
    }
}
