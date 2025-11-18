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
        
        public int CuentaId { get; set; }

        public string NumeroCuenta { get; set; } = string.Empty;

        public TipoCuentaEnum TipoCuenta { get; set; }

        public decimal SaldoInicial { get; set; }

        public bool Estado { get; set; } = true;

        public int ClienteId { get; set; }

        public virtual Cliente Cliente { get; set; } = null!;
      
        public DateTime? FechaActualizacion { get; set; }

        public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    }
    public enum TipoCuentaEnum
    {
        Ahorros,
        Corriente
    }

}
