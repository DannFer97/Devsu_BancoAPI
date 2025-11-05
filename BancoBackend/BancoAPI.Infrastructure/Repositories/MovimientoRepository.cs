using Microsoft.EntityFrameworkCore;
using BancoAPI.Domain.Entities;
using BancoAPI.Domain.Interfaces;
using BancoAPI.Infrastructure.Data;

namespace BancoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación específica del repositorio de Movimiento
    /// </summary>
    public class MovimientoRepository : Repository<Movimiento>, IMovimientoRepository
    {
        public MovimientoRepository(BancoDbContext context) : base(context)
        {
        }

        /// Obtiene los movimientos de una cuenta en un rango de fechas
        public async Task<IEnumerable<Movimiento>> GetMovimientosByFechaRangoAsync(
            int cuentaId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            return await _dbSet
                .Include(m => m.Cuenta)
                    .ThenInclude(c => c.Cliente)
                .Where(m => m.CuentaId == cuentaId &&
                           m.Fecha >= fechaInicio &&
                           m.Fecha <= fechaFin)
                .OrderBy(m => m.Fecha)
                .ToListAsync();
        }


        /// Obtiene el total de débitos (retiros) de una cuenta en un día específico
        /// Para validar el límite diario de $1000
        public async Task<decimal> GetTotalDebitosDelDiaAsync(int cuentaId, DateTime fecha)
        {
            var inicioDia = fecha.Date;
            var finDia = fecha.Date.AddDays(1).AddTicks(-1);

            // Suma todos los valores negativos (débitos) del día
            var totalDebitos = await _dbSet
                .Where(m => m.CuentaId == cuentaId &&
                           m.Fecha >= inicioDia &&
                           m.Fecha <= finDia &&
                           m.Valor < 0) 
                .SumAsync(m => Math.Abs(m.Valor)); 

            return totalDebitos;
        }


        /// Obtiene los movimientos de un cliente en un rango de fechas (para reportes)
        public async Task<IEnumerable<Movimiento>> GetMovimientosByClienteYFechaAsync(
            int clienteId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            return await _dbSet
                .Include(m => m.Cuenta)
                    .ThenInclude(c => c.Cliente)
                .Where(m => m.Cuenta.ClienteId == clienteId &&
                           m.Fecha >= fechaInicio &&
                           m.Fecha <= fechaFin)
                .OrderBy(m => m.Fecha)
                .ToListAsync();
        }

        /// Override de GetAllAsync para incluir cuenta y cliente
        public override async Task<IEnumerable<Movimiento>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.Cuenta)
                    .ThenInclude(c => c.Cliente)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        /// Override de GetByIdAsync para incluir cuenta y cliente
        public override async Task<Movimiento?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Cuenta)
                    .ThenInclude(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
        }

        /// Obtiene el último movimiento de una cuenta (para obtener saldo actual)
        public async Task<Movimiento?> GetUltimoMovimientoPorCuentaAsync(int cuentaId)
        {
            return await _dbSet
                .Where(m => m.CuentaId == cuentaId)
                .OrderByDescending(m => m.Fecha)
                .ThenByDescending(m => m.MovimientoId)
                .FirstOrDefaultAsync();
        }
    }
}