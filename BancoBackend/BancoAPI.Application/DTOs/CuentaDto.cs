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
    /// DTO para crear una nueva cuenta
    /// </summary>
    public class CuentaCreateDto
    {
        [Required(ErrorMessage = "El número de cuenta es obligatorio")]
        [StringLength(20, ErrorMessage = "El número de cuenta no puede superar los 20 caracteres")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "El número de cuenta solo debe contener números")]
        public string NumeroCuenta { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de cuenta es obligatorio")]
        [EnumDataType(typeof(TipoCuentaEnum))]
        public string TipoCuenta { get; set; } = string.Empty;


        [Required(ErrorMessage = "El saldo inicial es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El saldo inicial no puede ser negativo")]
        public decimal SaldoInicial { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int ClienteId { get; set; }

        public bool Estado { get; set; } = true;
    }

    /// <summary>
    /// DTO para actualizar una cuenta existente
    /// </summary>
    public class CuentaUpdateDto
    {
        [StringLength(20, ErrorMessage = "El tipo de cuenta no puede exceder 20 caracteres")]
        public string? TipoCuenta { get; set; }

        public bool? Estado { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para Cuenta
    /// </summary>
    public class CuentaDto
    {
        public int CuentaId { get; set; }
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public decimal SaldoActual { get; set; } // Saldo del último movimiento, o SaldoInicial si no hay movimientos
        public bool Estado { get; set; }
        public int ClienteId { get; set; }
        public string? NombreCliente { get; set; }
        public List<MovimientoDto>? Movimientos { get; set; }
    }
}
