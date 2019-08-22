using Models;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Impls
{
    public class ClienteOpcionesService : IClienteOpcionesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<ClienteOpciones> repository;
        public ClienteOpcionesService(ApplicationDbContext context)
        {
            _context = context;
            repository = new Repository<ClienteOpciones>(context);
        }

        public async Task InicializarUsuario(string UserId)
        {
            ClienteOpciones opciones = new ClienteOpciones(UserId);
            await repository.AddAsync(opciones);
            await repository.SaveChangesAsync();
        }

        public async Task<ClienteOpciones> GetOpciones(string UserId)
        {
            return await repository.FindAsync(o => o.UserId == UserId);
        }

        public async Task<bool> TemporizadorStatus(string UserId)
        {
            return (await repository.FindAsync(o => o.UserId == UserId)).TemporizadoresUserEnable;
        }

        public async Task<bool> TemporizadorStatus(string UserId, bool status)
        {
            ClienteOpciones op = await repository.FindAsync(o => o.UserId == UserId);
            op.TemporizadoresUserEnable = status;

            await repository.UpdateAsync(op, op.Id);
            await repository.SaveChangesAsync();

            return status;
        }

        
    }
}
