using BancoAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Application.Interfaces
{
    /// <summary>
    /// Interfaz del servicio de Movimiento
    /// Incluye validaciones 
    /// </summary>
    public interface IMovimientoService
    {
        Task<IEnumerable<MovimientoDto>> GetAllMovimientosAsync();
        Task<MovimientoDto?> GetMovimientoByIdAsync(int id);
        Task<IEnumerable<MovimientoDto>> GetMovimientosByCuentaAsync(int cuentaId);

        
        Task<MovimientoDto> CreateMovimientoAsync(MovimientoCreateDto movimientoDto);

        Task<bool> DeleteMovimientoAsync(int id);

        Task<ReporteMovimientosDto> GetReporteMovimientosAsync(
            int clienteId,
            DateTime fechaInicio,
            DateTime fechaFin);
    }
}
