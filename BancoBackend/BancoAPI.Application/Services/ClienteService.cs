using AutoMapper;
using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using BancoAPI.Domain.Entities;
using BancoAPI.Domain.Exceptions;
using BancoAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Application.Services
{
    /// <summary>
    /// Servicios de Cliente

    /// </summary>
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClienteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        public async Task<IEnumerable<ClienteDto>> GetAllClientesAsync()
        {
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }

        /// <summary>
        /// Obtiene un cliente por ID
        /// </summary>
        public async Task<ClienteDto?> GetClienteByIdAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);

            if (cliente == null)
                throw new EntidadNoEncontradaException("Cliente", id);

            return _mapper.Map<ClienteDto>(cliente);
        }

        /// <summary>
        /// Obtiene un cliente por identificación
        /// </summary>
        public async Task<ClienteDto?> GetClienteByIdentificacionAsync(string identificacion)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdentificacionAsync(identificacion);
            return cliente != null ? _mapper.Map<ClienteDto>(cliente) : null;
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// Valida que no exista duplicado por identificación
        /// </summary>
        public async Task<ClienteDto> CreateClienteAsync(ClienteCreateDto clienteDto)
        {
            // Validar que no exista un cliente con la misma identificación
            var existeCliente = await _unitOfWork.Clientes
                .AnyAsync(c => c.Identificacion == clienteDto.Identificacion);

            if (existeCliente)
                throw new EntidadDuplicadaException("Cliente", "Identificacion", clienteDto.Identificacion);

            var cliente = _mapper.Map<Cliente>(clienteDto);

            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ClienteDto>(cliente);
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// Solo actualiza los campos que no son null en el DTO
        /// </summary>
        public async Task<ClienteDto> UpdateClienteAsync(int id, ClienteUpdateDto clienteDto)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);

            if (cliente == null)
                throw new EntidadNoEncontradaException("Cliente", id);

            // Actualizar solo los campos que no son null 
            if (clienteDto.Nombre != null)
                cliente.Nombre = clienteDto.Nombre;

            if (clienteDto.Genero != null)
                cliente.Genero = clienteDto.Genero;

            if (clienteDto.Edad.HasValue)
                cliente.Edad = clienteDto.Edad.Value;

            if (clienteDto.Direccion != null)
                cliente.Direccion = clienteDto.Direccion;

            if (clienteDto.Telefono != null)
                cliente.Telefono = clienteDto.Telefono;

            if (clienteDto.Contrasena != null)
                cliente.Contrasena = clienteDto.Contrasena;

            if (clienteDto.Estado.HasValue)
                cliente.Estado = clienteDto.Estado.Value;
           

            await _unitOfWork.Clientes.UpdateAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ClienteDto>(cliente);
        }

        /// <summary>
        /// Elimina un cliente
        /// Valida que no tenga cuentas asociadas
        /// </summary>
        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetClienteConCuentasAsync(id);

            if (cliente == null)
                throw new EntidadNoEncontradaException("Cliente", id);

            // Validar que no tenga cuentas activas
            if (cliente.Cuentas.Any(c => c.Estado))
                throw new OperacionInvalidaException(
                    "No se puede eliminar el cliente porque tiene cuentas activas asociadas");

            await _unitOfWork.Clientes.DeleteAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
