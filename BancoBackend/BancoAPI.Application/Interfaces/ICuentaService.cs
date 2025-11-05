using BancoAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Application.Interfaces
{
    /// <summary>
    /// Interfaz del servicio de Cuenta
    /// </summary>
    public interface ICuentaService
    {
        Task<IEnumerable<CuentaDto>> GetAllCuentasAsync();
        Task<CuentaDto?> GetCuentaByIdAsync(int id);
        Task<CuentaDto?> GetCuentaByNumeroAsync(string numeroCuenta);
        Task<IEnumerable<CuentaDto>> GetCuentasByClienteIdAsync(int clienteId);
        Task<CuentaDto> CreateCuentaAsync(CuentaCreateDto cuentaDto);
        Task<CuentaDto> UpdateCuentaAsync(int id, CuentaUpdateDto cuentaDto);
        Task<bool> DeleteCuentaAsync(int id);
    }
}
