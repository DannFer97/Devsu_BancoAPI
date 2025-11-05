using BancoAPI.Application.DTOs;

namespace BancoAPI.Application.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de generación de reportes PDF
    /// </summary>
    public interface IPdfReportService
    {
        
        byte[] GenerateMovimientosReportPdf(ReporteMovimientosDto reporte);
    }
}