using BancoAPI.API.Controllers;
using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using BancoAPI.Domain.Entities;
using BancoAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace BancoAPI.Tests
{
    /// <summary>
    /// Pruebas unitarias críticas para MovimientosController
    /// - Saldo disponible
    /// - Límite diario de retiros ($1000)
    /// - Validación de valores negativos/positivos según tipo de movimiento
    /// </summary>
    public class MovimientosControllerTests
    {
        private readonly Mock<IMovimientoService> _mockMovimientoService;
        private readonly Mock<ILogger<MovimientosController>> _mockLogger;
        private readonly MovimientosController _controller;

        public MovimientosControllerTests()
        {
            _mockMovimientoService = new Mock<IMovimientoService>();
            _mockLogger = new Mock<ILogger<MovimientosController>>();
            _controller = new MovimientosController(_mockMovimientoService.Object, _mockLogger.Object);
        }

        #region Pruebas Críticas de Validación de Negocio

        /// Valida que no se pueda retirar más dinero del disponible
        [Fact]
        public async Task CreateMovimiento_WithInsufficientBalance_ThrowsSaldoInsuficienteException()
        {
            // Arrange
            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = TipoMovimientoEnum.Retiro,
                Valor = -500 // Intenta retirar 500
            };

            _mockMovimientoService
                .Setup(service => service.CreateMovimientoAsync(movimientoDto))
                .ThrowsAsync(new SaldoInsuficienteException(100, 500)); // Saldo actual: 100, necesita: 500

            // Act & Assert
            var exception = await Assert.ThrowsAsync<SaldoInsuficienteException>(
                async () => await _controller.CreateMovimiento(movimientoDto));

            exception.Message.Should().Contain("Saldo no disponible");
            exception.CodigoError.Should().Be("SALDO_INSUFICIENTE");
        }

        /// Valida el límite diario de retiros de $1000
        [Fact]
        public async Task CreateMovimiento_ExceedingDailyLimit_ThrowsCupoDiarioExcedidoException()
        {
            // Arrange
            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = TipoMovimientoEnum.Retiro,
                Valor = -600 // Intenta retirar 600 adicionales
            };

            _mockMovimientoService
                .Setup(service => service.CreateMovimientoAsync(movimientoDto))
                .ThrowsAsync(new CupoDiarioExcedidoException(500, 1000)); // Ya retiró 500, límite 1000

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CupoDiarioExcedidoException>(
                async () => await _controller.CreateMovimiento(movimientoDto));

            exception.Message.Should().Contain("Cupo diario excedido");
            exception.CodigoError.Should().Be("CUPO_DIARIO_EXCEDIDO");
        }

        /// Valida que los retiros se procesen correctamente con valor negativo
        [Fact]
        public async Task CreateMovimiento_ValidWithdrawal_ReturnsCreatedResult()
        {
            // Arrange
            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = TipoMovimientoEnum.Retiro,
                Valor = -100 // Retiro de 100 (valor negativo según documento)
            };

            var createdMovimiento = new MovimientoDto
            {
                MovimientoId = 1,
                CuentaId = 1,
                Fecha = DateTime.Now,
                TipoMovimiento = "Retiro",
                Valor = -100,
                Saldo = 900 // Saldo resultante después del retiro
            };

            _mockMovimientoService
                .Setup(service => service.CreateMovimientoAsync(movimientoDto))
                .ReturnsAsync(createdMovimiento);

            // Act
            var result = await _controller.CreateMovimiento(movimientoDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(MovimientosController.GetMovimiento));

            var returnedMovimiento = createdResult.Value.Should().BeOfType<MovimientoDto>().Subject;
            returnedMovimiento.Valor.Should().Be(-100);
            returnedMovimiento.Saldo.Should().Be(900);
            returnedMovimiento.TipoMovimiento.Should().Be("Retiro");
        }

        /// Valida que los depósitos se procesen correctamente con valor positivo

        [Fact]
        public async Task CreateMovimiento_ValidDeposit_ReturnsCreatedResult()
        {
            // Arrange
            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = TipoMovimientoEnum.Deposito,
                Valor = 500 // Depósito de 500 (valor positivo según documento)
            };

            var createdMovimiento = new MovimientoDto
            {
                MovimientoId = 2,
                CuentaId = 1,
                Fecha = DateTime.Now,
                TipoMovimiento = "Deposito",
                Valor = 500,
                Saldo = 1500 // Saldo resultante después del depósito
            };

            _mockMovimientoService
                .Setup(service => service.CreateMovimientoAsync(movimientoDto))
                .ReturnsAsync(createdMovimiento);

            // Act
            var result = await _controller.CreateMovimiento(movimientoDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;

            var returnedMovimiento = createdResult.Value.Should().BeOfType<MovimientoDto>().Subject;
            returnedMovimiento.Valor.Should().Be(500);
            returnedMovimiento.Saldo.Should().Be(1500);
            returnedMovimiento.TipoMovimiento.Should().Be("Deposito");
        }

        /// Valida que no se puedan hacer movimientos en cuenta inactiva
        [Fact]
        public async Task CreateMovimiento_OnInactiveAccount_ThrowsOperacionInvalidaException()
        {
            // Arrange
            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 999,
                TipoMovimiento = TipoMovimientoEnum.Deposito,
                Valor = 100
            };

            _mockMovimientoService
                .Setup(service => service.CreateMovimientoAsync(movimientoDto))
                .ThrowsAsync(new OperacionInvalidaException("La cuenta no está activa"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OperacionInvalidaException>(
                async () => await _controller.CreateMovimiento(movimientoDto));

            exception.Message.Should().Contain("no está activa");
            exception.CodigoError.Should().Be("OPERACION_INVALIDA");
        }

        #endregion

        #region Pruebas de Validación de Entrada

        /// Valida que se rechacen movimientos con modelo inválido
        [Fact]
        public async Task CreateMovimiento_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 0, // CuentaId inválido
                TipoMovimiento = TipoMovimientoEnum.Retiro,
                Valor = -100
            };

            _controller.ModelState.AddModelError("CuentaId", "La cuenta es obligatoria");

            // Act
            var result = await _controller.CreateMovimiento(movimientoDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            _mockMovimientoService.Verify(
                service => service.CreateMovimientoAsync(It.IsAny<MovimientoCreateDto>()),
                Times.Never);
        }

        #endregion

        #region Pruebas Básicas de CRUD

        [Fact]
        public async Task GetMovimientos_ReturnsOkResult()
        {
            // Arrange
            var movimientos = new List<MovimientoDto>
            {
                new MovimientoDto { MovimientoId = 1, Valor = -100, Saldo = 900 },
                new MovimientoDto { MovimientoId = 2, Valor = 500, Saldo = 1400 }
            };

            _mockMovimientoService
                .Setup(service => service.GetAllMovimientosAsync())
                .ReturnsAsync(movimientos);

            // Act
            var result = await _controller.GetMovimientos();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedMovimientos = okResult.Value.Should().BeAssignableTo<IEnumerable<MovimientoDto>>().Subject;
            returnedMovimientos.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetMovimiento_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var movimientoId = 1;
            var movimiento = new MovimientoDto
            {
                MovimientoId = movimientoId,
                Valor = -100,
                Saldo = 900
            };

            _mockMovimientoService
                .Setup(service => service.GetMovimientoByIdAsync(movimientoId))
                .ReturnsAsync(movimiento);

            // Act
            var result = await _controller.GetMovimiento(movimientoId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedMovimiento = okResult.Value.Should().BeOfType<MovimientoDto>().Subject;
            returnedMovimiento.MovimientoId.Should().Be(movimientoId);
        }

        #endregion
    }
}
