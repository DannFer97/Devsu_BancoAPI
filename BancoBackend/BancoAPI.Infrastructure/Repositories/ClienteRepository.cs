using Microsoft.EntityFrameworkCore;
using BancoAPI.Domain.Entities;
using BancoAPI.Domain.Interfaces;
using BancoAPI.Infrastructure.Data;

namespace BancoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación específica del repositorio de Cliente
    /// </summary>
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(BancoDbContext context) : base(context)
        {
        }

        /// Busca un cliente por su identificación
        public async Task<Cliente?> GetByIdentificacionAsync(string identificacion)
        {
            return await _dbSet
                .Include(c => c.Cuentas)
                    .ThenInclude(cu => cu.Movimientos)
                .FirstOrDefaultAsync(c => c.Identificacion == identificacion);
        }

        /// Obtiene un cliente con sus cuentas asociadas (eager loading)
        public async Task<Cliente?> GetClienteConCuentasAsync(int clienteId)
        {
            return await _dbSet
                .Include(c => c.Cuentas)
                .FirstOrDefaultAsync(c => c.PersonaId == clienteId);
        }

        /// Override de GetAllAsync para incluir las cuentas y movimientos
        public override async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Cuentas)
                    .ThenInclude(cu => cu.Movimientos)
                .ToListAsync();
        }

        /// Override de GetByIdAsync para incluir las cuentas y movimientos
        public override async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Cuentas)
                    .ThenInclude(cu => cu.Movimientos)
                .FirstOrDefaultAsync(c => c.PersonaId == id);
        }
    }
}