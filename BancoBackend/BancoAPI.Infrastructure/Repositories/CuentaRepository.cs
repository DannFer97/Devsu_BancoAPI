using BancoAPI.Domain.Entities;
using BancoAPI.Domain.Interfaces;
using BancoAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BancoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación específica del repositorio de Cuenta
    /// </summary>
    public class CuentaRepository : Repository<Cuenta>, ICuentaRepository
    {
        public CuentaRepository(BancoDbContext context) : base(context)
        {
        }

        /// Override de GetAllAsync para incluir cliente y movimientos
        public override async Task<IEnumerable<Cuenta>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .Include(c => c.Movimientos)
                .ToListAsync();
        }

        /// Busca una cuenta por su número de cuenta
        public async Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta)
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .Include(c => c.Movimientos)
                .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
        }

        /// Obtiene todas las cuentas de un cliente específico
        public async Task<IEnumerable<Cuenta>> GetCuentasByClienteIdAsync(int clienteId)
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .Include(c => c.Movimientos)
                .Where(c => c.ClienteId == clienteId)
                .ToListAsync();
        }

        /// Obtiene una cuenta con sus movimientos (eager loading)
        public async Task<Cuenta?> GetCuentaConMovimientosAsync(int cuentaId)
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .Include(c => c.Movimientos.OrderByDescending(m => m.Fecha))
                .FirstOrDefaultAsync(c => c.CuentaId == cuentaId);
        }

        /// Override de GetByIdAsync para incluir cliente y movimientos
        public override async Task<Cuenta?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .Include(c => c.Movimientos)
                .FirstOrDefaultAsync(c => c.CuentaId == id);
        }
    }
}
