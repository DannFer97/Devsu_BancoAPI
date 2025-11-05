using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BancoAPI.API.Controllers
{
    /// <summary>
    /// Controller REST para generación de Reportes
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IMovimientoService _movimientoService;
        private readonly IPdfReportService _pdfReportService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IMovimientoService movimientoService, IPdfReportService pdfReportService, ILogger<ReportesController> logger)
        {
            _movimientoService = movimientoService;
            _pdfReportService = pdfReportService;
            _logger = logger;
        }

        /// <summary>
        /// Genera un reporte de estado de cuenta por cliente y rango de fechas
        /// 

        [HttpGet]
        [ProducesResponseType(typeof(ReporteMovimientosDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReporteMovimientosDto>> GetReporte(
            [FromQuery] int clienteId,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            _logger.LogInformation(
                "GET /api/reportes - ClienteId: {ClienteId}, FechaInicio: {FechaInicio}, FechaFin: {FechaFin}",
                clienteId, fechaInicio, fechaFin);

            // Validaciones
            if (clienteId <= 0)
                return BadRequest(new { error = "El clienteId debe ser mayor a 0" });

            if (fechaFin < fechaInicio)
                return BadRequest(new { error = "La fecha fin debe ser mayor o igual a la fecha inicio" });

            // Generar reporte
            var reporte = await _movimientoService.GetReporteMovimientosAsync(
                clienteId,
                fechaInicio,
                fechaFin);

            return Ok(reporte);
        }

        /// <summary>
        /// Genera un reporte de estado de cuenta en formato PDF para descargar
        /// </summary>
        /// 
        [HttpGet("pdf")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetReportePdf(
            [FromQuery] int clienteId,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            _logger.LogInformation(
                "GET /api/reportes/pdf - ClienteId: {ClienteId}, FechaInicio: {FechaInicio}, FechaFin: {FechaFin}",
                clienteId, fechaInicio, fechaFin);

            // Validaciones
            if (clienteId <= 0)
            {
                _logger.LogWarning("ClienteId inválido: {ClienteId}", clienteId);
                return BadRequest(new { error = "El clienteId debe ser mayor a 0" });
            }

            if (fechaFin < fechaInicio)
            {
                _logger.LogWarning("Rango de fechas inválido: {FechaInicio} a {FechaFin}", fechaInicio, fechaFin);
                return BadRequest(new { error = "La fecha fin debe ser mayor o igual a la fecha inicio" });
            }

            // Validación adicional: No permitir rangos de tiempo muy extensos para evitar problemas de rendimiento
            var diasRango = (fechaFin - fechaInicio).TotalDays;
            if (diasRango > 365) // Máximo 1 año de rango
            {
                _logger.LogWarning("Rango de fechas excede el límite permitido: {DiasRango} días", diasRango);
                return BadRequest(new { error = "El rango de fechas no debe exceder un año" });
            }

            try
            {
                // Generar reporte
                var reporte = await _movimientoService.GetReporteMovimientosAsync(
                    clienteId,
                    fechaInicio,
                    fechaFin);

                if (reporte == null || reporte.Movimientos == null || !reporte.Movimientos.Any())
                {
                    _logger.LogWarning("No se encontraron movimientos para el cliente {ClienteId} en el rango {FechaInicio} a {FechaFin}", clienteId, fechaInicio, fechaFin);
                    return NotFound(new { error = "No se encontraron movimientos para los parámetros especificados" });
                }

                // Generar PDF
                var pdfBytes = _pdfReportService.GenerateMovimientosReportPdf(reporte);

                
                var fileName = $"reporte_movimientos_{clienteId}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf";
                fileName = SanitizeFileName(fileName); 

                _logger.LogInformation("PDF generado exitosamente para cliente {ClienteId}, tamaño: {TamañoBytes} bytes", clienteId, pdfBytes.Length);

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF para cliente {ClienteId}", clienteId);
                return StatusCode(500, new { error = "Error interno al generar el reporte PDF" });
            }
        }

        /// <summary>
        /// Sanitiza el nombre de archivo para prevenir ataques de path traversal
        /// </summary>
        private static string SanitizeFileName(string fileName)
        {
            // Eliminar caracteres potencialmente peligrosos
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            
            // Asegurar extensión .pdf
            if (!sanitized.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                sanitized += ".pdf";
            }

            return sanitized;
        }
    }
}
