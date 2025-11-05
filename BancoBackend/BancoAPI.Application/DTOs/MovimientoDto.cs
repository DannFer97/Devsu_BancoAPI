using BancoAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Application.DTOs
{
    /// <summary>
    /// DTO para crear un nuevo movimiento
    /// </summary>
    public class MovimientoCreateDto
    {
        [Required(ErrorMessage = "La cuenta es obligatoria")]
        public int CuentaId { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio")]
        public TipoMovimientoEnum TipoMovimiento { get; set; }

        [Required(ErrorMessage = "El valor es obligatorio")]
        public decimal Valor { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para Movimiento
    /// </summary>
    public class MovimientoDto
    {
        public int MovimientoId { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }
        public int CuentaId { get; set; }
        public string? NumeroCuenta { get; set; }
        public string? NombreCliente { get; set; }
    }

    /// <summary>
    /// DTO para el reporte de estado de cuenta
    /// </summary>
    public class EstadoCuentaDto
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string NumeroCuenta { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }
        public decimal Movimiento { get; set; }
        public decimal SaldoDisponible { get; set; }
    }

    /// <summary>
    /// DTO para el reporte de movimientos por cliente y fecha
    /// </summary>
    public class ReporteMovimientosDto
    {
        public string Cliente { get; set; } = string.Empty;
        public List<EstadoCuentaDto> Movimientos { get; set; } = new();
        public decimal TotalCreditos { get; set; }
        public decimal TotalDebitos { get; set; }
    }
}
