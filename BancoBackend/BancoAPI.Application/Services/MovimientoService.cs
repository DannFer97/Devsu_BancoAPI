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
    /// Servicios de Movimiento 
    /// - Saldo disponible
    /// - Límite diario de retiro ($1000)
    /// </summary>
    public class MovimientoService : IMovimientoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const decimal LIMITE_DIARIO_RETIRO = 1000m;

        public MovimientoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MovimientoDto>> GetAllMovimientosAsync()
        {
            var movimientos = await _unitOfWork.Movimientos.GetAllAsync();
            return _mapper.Map<IEnumerable<MovimientoDto>>(movimientos);
        }

        public async Task<MovimientoDto?> GetMovimientoByIdAsync(int id)
        {
            var movimiento = await _unitOfWork.Movimientos.GetByIdAsync(id);

            if (movimiento == null)
                throw new EntidadNoEncontradaException("Movimiento", id);

            return _mapper.Map<MovimientoDto>(movimiento);
        }

        public async Task<IEnumerable<MovimientoDto>> GetMovimientosByCuentaAsync(int cuentaId)
        {
            var cuenta = await _unitOfWork.Cuentas.GetCuentaConMovimientosAsync(cuentaId);

            if (cuenta == null)
                throw new EntidadNoEncontradaException("Cuenta", cuentaId);

            return _mapper.Map<IEnumerable<MovimientoDto>>(cuenta.Movimientos);
        }

        /// <summary>
        /// Crea un movimiento 
        /// 1. Valida saldo disponible
        /// 2. Valida límite diario de retiro ($1000)
        /// 3. Calcula saldo resultante
        /// </summary>
        public async Task<MovimientoDto> CreateMovimientoAsync(MovimientoCreateDto movimientoDto)

        {
            // 1. Obtener cuenta con movimientos
            var cuenta = await _unitOfWork.Cuentas.GetCuentaConMovimientosAsync(movimientoDto.CuentaId);

            if (cuenta == null)
                throw new EntidadNoEncontradaException("Cuenta", movimientoDto.CuentaId);

            if (!cuenta.Estado)
                throw new OperacionInvalidaException("La cuenta no está activa");

            // 2. Calcular saldo actual
            var saldoActual = cuenta.Movimientos
                .OrderByDescending(m => m.Fecha)
                .Select(m => m.Saldo)
                .FirstOrDefault(cuenta.SaldoInicial);


            // 3. Validar tipo de movimiento y valores según documento:
            // - Crédito (Deposito): valores positivos
            // - Débito (Retiro): valores negativos
            var esRetiro = movimientoDto.TipoMovimiento == TipoMovimientoEnum.Retiro;
            var esDeposito = movimientoDto.TipoMovimiento == TipoMovimientoEnum.Deposito;

            if (esRetiro)
            {
                // Validar que el retiro sea negativo (según documento)
                if (movimientoDto.Valor >= 0)
                    throw new OperacionInvalidaException("El valor del retiro debe ser negativo");

                if (saldoActual == 0)
                    throw new SaldoInsuficienteException();

                // Usar valor absoluto para comparar con saldo disponible
                var montoRetiro = Math.Abs(movimientoDto.Valor);

                if (montoRetiro > saldoActual)
                    throw new SaldoInsuficienteException(saldoActual, montoRetiro);

                // 4. Validar límite diario ($1000)
                var totalRetiradoHoy = await _unitOfWork.Movimientos
                    .GetTotalDebitosDelDiaAsync(movimientoDto.CuentaId, DateTime.Now);

                var nuevoTotalRetiros = totalRetiradoHoy + montoRetiro;

                if (nuevoTotalRetiros > LIMITE_DIARIO_RETIRO)
                    throw new CupoDiarioExcedidoException(totalRetiradoHoy, LIMITE_DIARIO_RETIRO);
            }
            else if (esDeposito)
            {
                // Validar que el depósito sea positivo (según documento)
                if (movimientoDto.Valor <= 0)
                    throw new OperacionInvalidaException("El valor del depósito debe ser positivo");
            }

            // 6. Calcular nuevo saldo
            var nuevoSaldo = saldoActual + movimientoDto.Valor;

            // 7. Crear movimiento
            var movimiento = new Movimiento
            {
                CuentaId = movimientoDto.CuentaId,
                Fecha = DateTime.Now,
                TipoMovimiento = movimientoDto.TipoMovimiento,
                Valor = movimientoDto.Valor,
                Saldo = nuevoSaldo
            };

            await _unitOfWork.Movimientos.AddAsync(movimiento);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MovimientoDto>(movimiento);
        }

        public async Task<bool> DeleteMovimientoAsync(int id)
        {
            var movimiento = await _unitOfWork.Movimientos.GetByIdAsync(id);

            if (movimiento == null)
                throw new EntidadNoEncontradaException("Movimiento", id);

            // Validar que sea el último movimiento de la cuenta
            var cuenta = await _unitOfWork.Cuentas.GetCuentaConMovimientosAsync(movimiento.CuentaId);
            var ultimoMovimiento = cuenta!.Movimientos
                .OrderByDescending(m => m.Fecha)
                .First();

            if (movimiento.MovimientoId != ultimoMovimiento.MovimientoId)
                throw new OperacionInvalidaException(
                    "Solo se puede eliminar el último movimiento de la cuenta");

            await _unitOfWork.Movimientos.DeleteAsync(movimiento);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Genera reporte de movimientos por cliente y rango de fechas
        /// </summary>
        public async Task<ReporteMovimientosDto> GetReporteMovimientosAsync(
            int clienteId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var cliente = await _unitOfWork.Clientes.GetClienteConCuentasAsync(clienteId);

            if (cliente == null)
                throw new EntidadNoEncontradaException("Cliente", clienteId);

            var movimientos = await _unitOfWork.Movimientos
                .GetMovimientosByClienteYFechaAsync(clienteId, fechaInicio, fechaFin);

          
            var estadosCuenta = movimientos.Select(m => new EstadoCuentaDto
            {
                Fecha = m.Fecha,
                Cliente = m.Cuenta.Cliente.Nombre,
                NumeroCuenta = m.Cuenta.NumeroCuenta,
                Tipo = m.Cuenta.TipoCuenta.ToString(),
                SaldoInicial = m.Cuenta.SaldoInicial,
                Estado = m.Cuenta.Estado,
                Movimiento = m.Valor,
                SaldoDisponible = m.Saldo
            }).ToList();

            // Calcular totales 
            var totalCreditos = movimientos
                .Where(m => m.Valor > 0)
                .Sum(m => m.Valor);

            var totalDebitos = Math.Abs(movimientos
                .Where(m => m.Valor < 0)
                .Sum(m => m.Valor));

            return new ReporteMovimientosDto
            {
                Cliente = cliente.Nombre,
                Movimientos = estadosCuenta,
                TotalCreditos = totalCreditos,
                TotalDebitos = totalDebitos
            };
        }
    }
}
