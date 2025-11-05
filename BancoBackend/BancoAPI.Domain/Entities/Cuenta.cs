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
    /// Entidad que representa una cuenta bancaria
    /// </summary>
    public class Cuenta
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CuentaId { get; set; }

    
        [Required(ErrorMessage = "El número de cuenta es obligatorio")]
        [StringLength(20, ErrorMessage = "El número de cuenta no puede exceder 20 caracteres")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "El número de cuenta solo debe contener números")]
        public string NumeroCuenta { get; set; } = string.Empty;

    
        [Required(ErrorMessage = "El tipo de cuenta es obligatorio")]
        public TipoCuentaEnum TipoCuenta { get; set; }

        
        [Required(ErrorMessage = "El saldo inicial es obligatorio")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El saldo inicial no puede ser negativo")]
        public decimal SaldoInicial { get; set; }

        
        [Required(ErrorMessage = "El estado es obligatorio")]
        public bool Estado { get; set; } = true;

        
        [Required(ErrorMessage = "El cliente es obligatorio")]
        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }

        
        public virtual Cliente? Cliente { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;


        [DataType(DataType.DateTime)]
        public DateTime? FechaActualizacion { get; set; }

        public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    }
    public enum TipoCuentaEnum
    {
        Ahorros,
        Corriente
    }

}
