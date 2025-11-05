using BancoAPI.Application.Configuration;
using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BancoAPI.Application.Services
{
    /// <summary>
    /// Servicio para generación de reportes PDF con mejores prácticas modernas
    ///
    /// </summary>
    public class PdfReportService : IPdfReportService
    {
        private readonly ILogger<PdfReportService> _logger;
        private readonly PdfConfiguration _config;

        static PdfReportService()
        {
            // Configurar licencia de QuestPDF 
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public PdfReportService(
            ILogger<PdfReportService> logger,
            IOptions<PdfConfiguration> config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Genera un PDF del reporte de movimientos para un cliente
        /// </summary>
        
        public byte[] GenerateMovimientosReportPdf(ReporteMovimientosDto reporte)
        {
            
            if (reporte == null)
            {
                _logger.LogError("Intento de generar PDF con reporte nulo");
                throw new ArgumentNullException(nameof(reporte), "El reporte no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(reporte.Cliente))
            {
                _logger.LogWarning("Reporte sin nombre de cliente");
                throw new ArgumentException("El reporte debe tener un cliente especificado", nameof(reporte));
            }

            if (reporte.Movimientos == null || !reporte.Movimientos.Any())
            {
                _logger.LogWarning("Intento de generar PDF sin movimientos para cliente: {Cliente}", reporte.Cliente);
                throw new ArgumentException("El reporte debe contener al menos un movimiento", nameof(reporte));
            }

            try
            {
                _logger.LogInformation(
                    "Generando PDF para cliente: {Cliente}, Movimientos: {Count}",
                    reporte.Cliente,
                    reporte.Movimientos.Count());

                var document = CreatePdfDocument(reporte);

                using var stream = new MemoryStream();
                document.GeneratePdf(stream);
                var pdfBytes = stream.ToArray();

                _logger.LogInformation(
                    "PDF generado exitosamente. Tamaño: {Size} KB",
                    pdfBytes.Length / 1024);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al generar PDF para cliente: {Cliente}",
                    reporte.Cliente);

                throw new InvalidOperationException(
                    $"Error al generar el reporte PDF: {ex.Message}",
                    ex);
            }
        }

        /// <summary>
        /// Crea el documento PDF con toda la estructura
        /// </summary>
        private IDocument CreatePdfDocument(ReporteMovimientosDto reporte)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configuración de página
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(_config.Fonts.NormalSize));

                    // Encabezado
                    page.Header().Component(new ReportHeader(reporte, _config));

                    // Contenido
                    page.Content().PaddingVertical(0.5f, Unit.Centimetre).Column(column =>
                    {
                        // Información del reporte
                        column.Item().Component(new ReportInfo(reporte, _config));

                        // Espacio
                        column.Item().PaddingBottom(15);

                        // Tabla de movimientos
                        column.Item().Component(new MovimientosTable(reporte, _config));

                        // Espacio
                        column.Item().PaddingTop(20);

                        // Totales
                        column.Item().Component(new ReportSummary(reporte, _config));
                    });

                    // Pie de página
                    page.Footer().Component(new ReportFooter(_config));
                });
            });

            return document;
        }
    }

    #region Components Reutilizables

    /// <summary>
    /// Componente: Encabezado del reporte
    /// </summary>
    internal class ReportHeader : IComponent
    {
        private readonly ReporteMovimientosDto _reporte;
        private readonly PdfConfiguration _config;

        public ReportHeader(ReporteMovimientosDto reporte, PdfConfiguration config)
        {
            _reporte = reporte;
            _config = config;
        }

        public void Compose(IContainer container)
        {
            container.Row(row =>
            {
                // Logo/Nombre de la empresa 
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(_config.Company.Name)
                        .FontSize(_config.Fonts.HeaderSize)
                        .SemiBold()
                        .FontColor(_config.Colors.Primary);

                    column.Item().Text("Estado de Cuenta")
                        .FontSize(_config.Fonts.SubHeaderSize)
                        .FontColor(_config.Colors.Secondary);
                });

                // Fecha de generación 
                row.RelativeItem().AlignRight().Column(column =>
                {
                    column.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}")
                        .FontSize(_config.Fonts.NormalSize);

                    column.Item().Text($"Hora: {DateTime.Now:HH:mm:ss}")
                        .FontSize(_config.Fonts.NormalSize);
                });
            });
        }
    }

    /// <summary>
    /// Componente: Información del reporte
    /// </summary>
    internal class ReportInfo : IComponent
    {
        private readonly ReporteMovimientosDto _reporte;
        private readonly PdfConfiguration _config;

        public ReportInfo(ReporteMovimientosDto reporte, PdfConfiguration config)
        {
            _reporte = reporte;
            _config = config;
        }

        public void Compose(IContainer container)
        {
            container.Background(_config.Colors.Light)
                .Padding(15)
                .Column(column =>
                {
                    column.Item().Text("Información del Cliente")
                        .SemiBold()
                        .FontSize(_config.Fonts.SubHeaderSize)
                        .FontColor(_config.Colors.Secondary);

                    column.Item().PaddingTop(10);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Cliente: ").SemiBold();
                            text.Span(_reporte.Cliente);
                        });

                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Total Movimientos: ").SemiBold();
                            text.Span(_reporte.Movimientos.Count().ToString());
                        });
                    });
                });
        }
    }

    /// <summary>
    /// Componente: Tabla de movimientos
    /// </summary>
    internal class MovimientosTable : IComponent
    {
        private readonly ReporteMovimientosDto _reporte;
        private readonly PdfConfiguration _config;

        public MovimientosTable(ReporteMovimientosDto reporte, PdfConfiguration config)
        {
            _reporte = reporte;
            _config = config;
        }

        public void Compose(IContainer container)
        {
            container.Table(table =>
            {
                // Definición de columnas
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);   // Fecha
                    columns.RelativeColumn(1.5f); // Número Cuenta
                    columns.RelativeColumn(1.2f); // Tipo
                    columns.RelativeColumn(1.3f); // Saldo Inicial
                    columns.RelativeColumn(1); // Estado
                    columns.RelativeColumn(1.3f); // Movimiento
                    columns.RelativeColumn(1.5f); // Saldo Disponible
                });

                // Encabezado de la tabla
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Fecha");
                    header.Cell().Element(HeaderCellStyle).Text("Nº Cuenta");
                    header.Cell().Element(HeaderCellStyle).Text("Tipo");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Saldo Inicial");
                    header.Cell().Element(HeaderCellStyle).Text("Estado");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Movimiento");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Saldo Final");
                });

                // Filas de datos
                foreach (var movimiento in _reporte.Movimientos)
                {
                    var rowNumber = _reporte.Movimientos.ToList().IndexOf(movimiento);
                    var isEvenRow = rowNumber % 2 == 0;

                    table.Cell().Element(c => DataCellStyle(c, isEvenRow))
                        .Text(movimiento.Fecha.ToString("dd/MM/yyyy HH:mm"))
                        .FontSize(_config.Fonts.SmallSize);

                    table.Cell().Element(c => DataCellStyle(c, isEvenRow))
                        .Text(movimiento.NumeroCuenta)
                        .FontSize(_config.Fonts.SmallSize);

                    table.Cell().Element(c => DataCellStyle(c, isEvenRow))
                        .Text(movimiento.Tipo)
                        .FontSize(_config.Fonts.SmallSize);

                    table.Cell().Element(c => DataCellStyle(c, isEvenRow))
                        .AlignRight()
                        .Text($"${movimiento.SaldoInicial:N2}")
                        .FontSize(_config.Fonts.SmallSize);

                    table.Cell().Element(c => DataCellStyle(c, isEvenRow))
                        .Text(movimiento.Estado ? "Activa" : "Inactiva")
                        .FontColor(movimiento.Estado ? _config.Colors.Success : _config.Colors.Danger)
                        .FontSize(_config.Fonts.SmallSize);

                    // Movimiento con color según tipo
                    table.Cell().Element(c => DataCellStyle(c, isEvenRow))
                        .AlignRight()
                        .Text($"{(movimiento.Movimiento >= 0 ? "+" : "")}${movimiento.Movimiento:N2}")
                        .FontColor(movimiento.Movimiento >= 0 ? _config.Colors.Success : _config.Colors.Danger)
                        .SemiBold()
                        .FontSize(_config.Fonts.SmallSize);

                    table.Cell().Element(c => DataCellStyle(c, isEvenRow))
                        .AlignRight()
                        .Text($"${movimiento.SaldoDisponible:N2}")
                        .SemiBold()
                        .FontSize(_config.Fonts.SmallSize);
                }
            });

            // Métodos helper para estilos
            IContainer HeaderCellStyle(IContainer container)
            {
                return container
                    .Background(_config.Colors.Secondary)
                    .Padding(8)
                    .DefaultTextStyle(x => x
                        .SemiBold()
                        .FontColor(Colors.White)
                        .FontSize(_config.Fonts.SmallSize));
            }

            IContainer DataCellStyle(IContainer container, bool isEvenRow)
            {
                return container
                    .Background(isEvenRow ? Colors.White : Colors.Grey.Lighten4)
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(6);
            }
        }
    }

    /// <summary>
    /// Componente: Resumen de totales
    /// </summary>
    internal class ReportSummary : IComponent
    {
        private readonly ReporteMovimientosDto _reporte;
        private readonly PdfConfiguration _config;

        public ReportSummary(ReporteMovimientosDto reporte, PdfConfiguration config)
        {
            _reporte = reporte;
            _config = config;
        }

        public void Compose(IContainer container)
        {
            container.Background(_config.Colors.Light)
                .Padding(15)
                .Column(column =>
                {
                    column.Item().Text("Resumen del Período")
                        .SemiBold()
                        .FontSize(_config.Fonts.SubHeaderSize)
                        .FontColor(_config.Colors.Secondary);

                    column.Item().PaddingTop(10);

                    // Grid de totales
                    column.Item().Row(row =>
                    {
                        // Total Créditos
                        row.RelativeItem().Background(Colors.White)
                            .Padding(10)
                            .Column(col =>
                            {
                                col.Item().Text("Total Créditos")
                                    .FontSize(_config.Fonts.SmallSize)
                                    .FontColor(Colors.Grey.Darken2);

                                col.Item().Text($"+${_reporte.TotalCreditos:N2}")
                                    .FontSize(_config.Fonts.SubHeaderSize)
                                    .SemiBold()
                                    .FontColor(_config.Colors.Success);
                            });

                        row.ConstantItem(10); // Espacio

                        // Total Débitos
                        row.RelativeItem().Background(Colors.White)
                            .Padding(10)
                            .Column(col =>
                            {
                                col.Item().Text("Total Débitos")
                                    .FontSize(_config.Fonts.SmallSize)
                                    .FontColor(Colors.Grey.Darken2);

                                col.Item().Text($"${_reporte.TotalDebitos:N2}")
                                    .FontSize(_config.Fonts.SubHeaderSize)
                                    .SemiBold()
                                    .FontColor(_config.Colors.Danger);
                            });

                        row.ConstantItem(10); // Espacio

                        // Balance Neto
                        row.RelativeItem().Background(_config.Colors.Secondary)
                            .Padding(10)
                            .Column(col =>
                            {
                                col.Item().Text("Balance Neto")
                                    .FontSize(_config.Fonts.SmallSize)
                                    .FontColor(Colors.White);

                                var neto = _reporte.TotalCreditos - _reporte.TotalDebitos;
                                col.Item().Text($"${neto:N2}")
                                    .FontSize(_config.Fonts.SubHeaderSize)
                                    .SemiBold()
                                    .FontColor(Colors.White);
                            });
                    });
                });
        }
    }

    /// <summary>
    /// Componente: Pie de página
    /// </summary>
    internal class ReportFooter : IComponent
    {
        private readonly PdfConfiguration _config;

        public ReportFooter(PdfConfiguration config)
        {
            _config = config;
        }

        public void Compose(IContainer container)
        {
            container.AlignCenter().Column(column =>
            {
                // Información de la empresa
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(_config.Company.Name)
                        .FontSize(_config.Fonts.SmallSize)
                        .SemiBold();

                    row.RelativeItem().AlignCenter().Text($"Tel: {_config.Company.Phone}")
                        .FontSize(_config.Fonts.SmallSize);

                    row.RelativeItem().AlignRight().Text(_config.Company.Website)
                        .FontSize(_config.Fonts.SmallSize);
                });

                // Número de página
                column.Item().PaddingTop(5).Text(text =>
                {
                    text.DefaultTextStyle(TextStyle.Default.FontSize(_config.Fonts.SmallSize));
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });

                // Aviso legal
                column.Item().PaddingTop(5).Text(
                    "Este documento ha sido generado automáticamente. Para cualquier consulta contacte a su ejecutivo.")
                    .FontSize(_config.Fonts.SmallSize - 1)
                    .Italic()
                    .FontColor(Colors.Grey.Darken1);
            });
        }
    }

    #endregion
}
