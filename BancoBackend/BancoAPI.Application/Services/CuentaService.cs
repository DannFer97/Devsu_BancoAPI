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
    /// Servicios de Cuenta 
    /// </summary>
    public class CuentaService : ICuentaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CuentaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CuentaDto>> GetAllCuentasAsync()
        {
            var cuentas = await _unitOfWork.Cuentas.GetAllAsync();
            var cuentasDto = _mapper.Map<IEnumerable<CuentaDto>>(cuentas).ToList();

            // Calcular el saldo actual de cada cuenta
            foreach (var cuentaDto in cuentasDto)
            {
                // Obtener el saldo actual del último movimiento
                var ultimoMovimiento = await _unitOfWork.Movimientos
                    .GetUltimoMovimientoPorCuentaAsync(cuentaDto.CuentaId);

                // Si hay movimientos, usar el saldo del último movimiento; si no, usar saldo inicial
                cuentaDto.SaldoActual = ultimoMovimiento?.Saldo ?? cuentaDto.SaldoInicial;
            }

            return cuentasDto;
        }

        public async Task<CuentaDto?> GetCuentaByIdAsync(int id)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(id);

            if (cuenta == null)
                throw new EntidadNoEncontradaException("Cuenta", id);

            var cuentaDto = _mapper.Map<CuentaDto>(cuenta);

            // Obtener el saldo actual del último movimiento
            var ultimoMovimiento = await _unitOfWork.Movimientos
                .GetUltimoMovimientoPorCuentaAsync(id);

            // Si hay movimientos, usar el saldo del último movimiento; si no, usar saldo inicial
            cuentaDto.SaldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;

            return cuentaDto;
        }

        public async Task<CuentaDto?> GetCuentaByNumeroAsync(string numeroCuenta)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);
            return cuenta != null ? _mapper.Map<CuentaDto>(cuenta) : null;
        }

        public async Task<IEnumerable<CuentaDto>> GetCuentasByClienteIdAsync(int clienteId)
        {
            var cuentas = await _unitOfWork.Cuentas.GetCuentasByClienteIdAsync(clienteId);
            return _mapper.Map<IEnumerable<CuentaDto>>(cuentas);
        }

        /// <summary>
        /// Crea una nueva cuenta
        /// Valida que el cliente exista y que no exista cuenta duplicada
        /// </summary>
        public async Task<CuentaDto> CreateCuentaAsync(CuentaCreateDto cuentaDto)
        {
            // Validar que el cliente existe
            var clienteExiste = await _unitOfWork.Clientes
                .AnyAsync(c => c.PersonaId == cuentaDto.ClienteId);

            if (!clienteExiste)
                throw new EntidadNoEncontradaException("Cliente", cuentaDto.ClienteId);

            // Validar que no exista una cuenta con el mismo número
            var existeCuenta = await _unitOfWork.Cuentas
                .AnyAsync(c => c.NumeroCuenta == cuentaDto.NumeroCuenta);

            if (existeCuenta)
                throw new EntidadDuplicadaException("Cuenta", "NumeroCuenta", cuentaDto.NumeroCuenta);
            

            var cuenta = _mapper.Map<Cuenta>(cuentaDto);

            await _unitOfWork.Cuentas.AddAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            // Recargar la cuenta con eager loading para incluir el cliente
            var cuentaCreada = await _unitOfWork.Cuentas.GetByIdAsync(cuenta.CuentaId);

            return _mapper.Map<CuentaDto>(cuentaCreada);
        }

        /// <summary>
        /// Actualiza una cuenta existente
        /// Solo permite actualizar tipo y estado
        /// </summary>
        public async Task<CuentaDto> UpdateCuentaAsync(int id, CuentaUpdateDto cuentaDto)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(id);

            if (cuenta == null)
                throw new EntidadNoEncontradaException("Cuenta", id);

            // Actualizar solo los campos permitidos
            if (!string.IsNullOrEmpty(cuentaDto.TipoCuenta))
            {
                if (Enum.TryParse<TipoCuentaEnum>(cuentaDto.TipoCuenta, true, out var tipoCuentaEnum))
                {
                    cuenta.TipoCuenta = tipoCuentaEnum;
                }
                else
                {
                    throw new OperacionInvalidaException($"Tipo de cuenta '{cuentaDto.TipoCuenta}' no es válido.");
                }
            }

            cuenta.FechaActualizacion = DateTime.Now;

            if (cuentaDto.Estado.HasValue)
                cuenta.Estado = cuentaDto.Estado.Value;

            await _unitOfWork.Cuentas.UpdateAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CuentaDto>(cuenta);
        }

        /// <summary>
        /// Elimina una cuenta
        /// Valida que no tenga movimientos asociados
        /// </summary>
        public async Task<bool> DeleteCuentaAsync(int id)
        {
            var cuenta = await _unitOfWork.Cuentas.GetCuentaConMovimientosAsync(id);

            if (cuenta == null)
                throw new EntidadNoEncontradaException("Cuenta", id);

            // Validar que no tenga movimientos
            if (cuenta.Movimientos.Any())
                throw new OperacionInvalidaException(
                    "No se puede eliminar la cuenta porque tiene movimientos asociados");

            await _unitOfWork.Cuentas.DeleteAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
