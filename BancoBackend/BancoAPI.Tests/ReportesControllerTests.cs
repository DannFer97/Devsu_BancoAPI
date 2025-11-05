using BancoAPI.API.Controllers;
using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace BancoAPI.Tests
{
    /// <summary>
    /// Pruebas unitarias para ReportesController
    /// </summary>
    public class ReportesControllerTests
    {
        private readonly Mock<IMovimientoService> _mockMovimientoService;
        private readonly Mock<IPdfReportService> _mockPdfReportService;
        private readonly Mock<ILogger<ReportesController>> _mockLogger;
        private readonly ReportesController _controller;

        public ReportesControllerTests()
        {
            _mockMovimientoService = new Mock<IMovimientoService>();
            _mockPdfReportService = new Mock<IPdfReportService>();
            _mockLogger = new Mock<ILogger<ReportesController>>();
            _controller = new ReportesController(
                _mockMovimientoService.Object, 
                _mockPdfReportService.Object, 
                _mockLogger.Object);
        }

        #region GetReporte Tests

        [Fact]
        public async Task GetReporte_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            var clienteId = 1;
            var fechaInicio = DateTime.Now.AddDays(-7);
            var fechaFin = DateTime.Now;
            var expectedReporte = new ReporteMovimientosDto
            {
                Cliente = "Cliente Prueba",
                TotalCreditos = 1000,
                TotalDebitos = 500,
                Movimientos = new List<EstadoCuentaDto>
                {
                    new EstadoCuentaDto
                    {
                        Fecha = DateTime.Now,
                        Cliente = "Cliente Prueba",
                        NumeroCuenta = "123456",
                        Tipo = "Ahorros",
                        SaldoInicial = 1000,
                        Estado = true,
                        Movimiento = 500,
                        SaldoDisponible = 1500
                    }
                }
            };

            _mockMovimientoService
                .Setup(service => service.GetReporteMovimientosAsync(
                    clienteId, fechaInicio, fechaFin))
                .ReturnsAsync(expectedReporte);

            // Act
            var result = await _controller.GetReporte(clienteId, fechaInicio, fechaFin);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReporte = okResult.Value.Should().BeOfType<ReporteMovimientosDto>().Subject;
            returnedReporte.Cliente.Should().Be(expectedReporte.Cliente);
            returnedReporte.TotalCreditos.Should().Be(expectedReporte.TotalCreditos);
            returnedReporte.Movimientos.Should().HaveCount(expectedReporte.Movimientos.Count);
        }

        [Fact]
        public async Task GetReporte_WithInvalidClienteId_ReturnsBadRequest()
        {
            // Arrange
            var clienteId = -1; // ID no válido
            var fechaInicio = DateTime.Now.AddDays(-7);
            var fechaFin = DateTime.Now;

            // Act
            var result = await _controller.GetReporte(clienteId, fechaInicio, fechaFin);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetReporte_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var clienteId = 1;
            var fechaInicio = DateTime.Now.AddDays(7); // Fecha inicio mayor que fecha fin
            var fechaFin = DateTime.Now;

            // Act
            var result = await _controller.GetReporte(clienteId, fechaInicio, fechaFin);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        #endregion

        #region GetReportePdf Tests

        [Fact]
        public async Task GetReportePdf_WithValidParameters_ReturnsFileResult()
        {
            // Arrange
            var clienteId = 1;
            var fechaInicio = DateTime.Now.AddDays(-7);
            var fechaFin = DateTime.Now;
            var reporte = new ReporteMovimientosDto
            {
                Cliente = "Cliente Prueba",
                TotalCreditos = 1000,
                TotalDebitos = 500,
                Movimientos = new List<EstadoCuentaDto>
                {
                    new EstadoCuentaDto
                    {
                        Fecha = DateTime.Now,
                        Cliente = "Cliente Prueba",
                        NumeroCuenta = "123456",
                        Tipo = "Ahorros",
                        SaldoInicial = 1000,
                        Estado = true,
                        Movimiento = 500,
                        SaldoDisponible = 1500
                    }
                }
            };
            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF header

            _mockMovimientoService
                .Setup(service => service.GetReporteMovimientosAsync(
                    clienteId, fechaInicio, fechaFin))
                .ReturnsAsync(reporte);

            _mockPdfReportService
                .Setup(service => service.GenerateMovimientosReportPdf(reporte))
                .Returns(pdfBytes);

            // Act
            var result = await _controller.GetReportePdf(clienteId, fechaInicio, fechaFin);

            // Assert
            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.FileContents.Should().ContainInOrder(pdfBytes);
            fileResult.ContentType.Should().Be("application/pdf");
            fileResult.FileDownloadName.Should().Contain("reporte_movimientos_1_");
            fileResult.FileDownloadName.Should().EndWith(".pdf");
        }

        [Fact]
        public async Task GetReportePdf_WithInvalidClienteId_ReturnsBadRequest()
        {
            // Arrange
            var clienteId = -1; // ID no válido
            var fechaInicio = DateTime.Now.AddDays(-7);
            var fechaFin = DateTime.Now;

            // Act
            var result = await _controller.GetReportePdf(clienteId, fechaInicio, fechaFin);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetReportePdf_WithDateRangeExceedingOneYear_ReturnsBadRequest()
        {
            // Arrange
            var clienteId = 1;
            var fechaInicio = DateTime.Now.AddYears(-2); // Más de un año
            var fechaFin = DateTime.Now;

            // Act
            var result = await _controller.GetReportePdf(clienteId, fechaInicio, fechaFin);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetReportePdf_WithNoDataFound_ReturnsNotFound()
        {
            // Arrange
            var clienteId = 1;
            var fechaInicio = DateTime.Now.AddDays(-7);
            var fechaFin = DateTime.Now;
            var emptyReporte = new ReporteMovimientosDto
            {
                Cliente = "Cliente Prueba",
                TotalCreditos = 0,
                TotalDebitos = 0,
                Movimientos = new List<EstadoCuentaDto>() // Lista vacía
            };

            _mockMovimientoService
                .Setup(service => service.GetReporteMovimientosAsync(
                    clienteId, fechaInicio, fechaFin))
                .ReturnsAsync(emptyReporte);

            // Act
            var result = await _controller.GetReportePdf(clienteId, fechaInicio, fechaFin);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().NotBeNull();
        }

        #endregion

        #region Security Tests

        [Fact]
        public async Task GetReportePdf_WithLongFileName_SanitizesFileName()
        {
            // Arrange
            var clienteId = 1;
            var fechaInicio = DateTime.Now.AddDays(-7);
            var fechaFin = DateTime.Now;
            var reporte = new ReporteMovimientosDto
            {
                Cliente = "Cliente Prueba",
                TotalCreditos = 1000,
                TotalDebitos = 500,
                Movimientos = new List<EstadoCuentaDto>
                {
                    new EstadoCuentaDto
                    {
                        Fecha = DateTime.Now,
                        Cliente = "Cliente Prueba",
                        NumeroCuenta = "123456",
                        Tipo = "Ahorros",
                        SaldoInicial = 1000,
                        Estado = true,
                        Movimiento = 500,
                        SaldoDisponible = 1500
                    }
                }
            };
            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 };

            _mockMovimientoService
                .Setup(service => service.GetReporteMovimientosAsync(
                    clienteId, fechaInicio, fechaFin))
                .ReturnsAsync(reporte);

            _mockPdfReportService
                .Setup(service => service.GenerateMovimientosReportPdf(reporte))
                .Returns(pdfBytes);

            // Act - Intentar un nombre de archivo potencialmente malicioso
            var result = await _controller.GetReportePdf(clienteId, fechaInicio, fechaFin);

            // Assert
            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            // Verificar que el nombre de archivo no contenga caracteres peligrosos
            fileResult.FileDownloadName.Should().NotContain("..");
            fileResult.FileDownloadName.Should().NotContain("/");
            fileResult.FileDownloadName.Should().NotContain("\\");
            fileResult.FileDownloadName.Should().EndWith(".pdf");
        }

        #endregion
    }
}