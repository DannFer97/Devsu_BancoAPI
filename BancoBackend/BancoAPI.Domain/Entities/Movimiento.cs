using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BancoAPI.Domain.Entities
{
    /// <summary>
    /// Entidad que representa un movimiento (transacción) bancario
    /// </summary>
    public class Movimiento
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovimientoId { get; set; }

    
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; } = DateTime.Now;

    
        [Required(ErrorMessage = "El tipo de movimiento es obligatorio")]
        public TipoMovimientoEnum TipoMovimiento { get; set; }


        [Required(ErrorMessage = "El valor es obligatorio")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }


        [Required(ErrorMessage = "El saldo es obligatorio")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El saldo no puede ser negativo")]
        public decimal Saldo { get; set; }


        [Required(ErrorMessage = "La cuenta es obligatoria")]
        [ForeignKey(nameof(Cuenta))]
        public int CuentaId { get; set; }

        public virtual Cuenta? Cuenta { get; set; }
    }

    public enum TipoMovimientoEnum
    {
        Deposito,
        Retiro
    }
}
