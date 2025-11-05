using BancoAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Application.Interfaces
{
    /// <summary>
    /// Interfaz del servicio de Cliente
    /// </summary>
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetAllClientesAsync();
        Task<ClienteDto?> GetClienteByIdAsync(int id);
        Task<ClienteDto?> GetClienteByIdentificacionAsync(string identificacion);
        Task<ClienteDto> CreateClienteAsync(ClienteCreateDto clienteDto);
        Task<ClienteDto> UpdateClienteAsync(int id, ClienteUpdateDto clienteDto);
        Task<bool> DeleteClienteAsync(int id);
    }
}
