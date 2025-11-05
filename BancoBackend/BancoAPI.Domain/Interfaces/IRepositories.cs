using BancoAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Domain.Interfaces
{
    /// <summary>
    /// Interfaz específica para el repositorio de Cliente
    /// </summary>
    public interface IClienteRepository : IRepository<Cliente>
    {

        /// Busca un cliente por su identificación

        Task<Cliente?> GetByIdentificacionAsync(string identificacion);


        /// Obtiene un cliente con sus cuentas asociadas

        Task<Cliente?> GetClienteConCuentasAsync(int clienteId);
    }

    /// <summary>
    /// Interfaz específica para el repositorio de Cuenta
    /// </summary>
    public interface ICuentaRepository : IRepository<Cuenta>
    {
       
        /// Busca una cuenta por su número de cuenta
        Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta);

        /// Obtiene todas las cuentas de un cliente
        Task<IEnumerable<Cuenta>> GetCuentasByClienteIdAsync(int clienteId);

        /// Obtiene una cuenta con sus movimientos
        Task<Cuenta?> GetCuentaConMovimientosAsync(int cuentaId);
    }

    /// <summary>
    /// Interfaz específica para el repositorio de Movimiento
    /// </summary>
    public interface IMovimientoRepository : IRepository<Movimiento>
    {

        /// Obtiene los movimientos de una cuenta en un rango de fechas

        Task<IEnumerable<Movimiento>> GetMovimientosByFechaRangoAsync(
            int cuentaId,
            DateTime fechaInicio,
            DateTime fechaFin);


        /// Obtiene el total de débitos (retiros) de una cuenta en un día específico
        /// Para validar el límite diario de $1000

        Task<decimal> GetTotalDebitosDelDiaAsync(int cuentaId, DateTime fecha);

        /// Obtiene los movimientos de un cliente en un rango de fechas (para reportes)

        Task<IEnumerable<Movimiento>> GetMovimientosByClienteYFechaAsync(
            int clienteId,
            DateTime fechaInicio,
            DateTime fechaFin);

        /// Obtiene el último movimiento de una cuenta (para obtener saldo actual)

        Task<Movimiento?> GetUltimoMovimientoPorCuentaAsync(int cuentaId);
    }
}
