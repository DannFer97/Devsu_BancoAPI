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

        public int MovimientoId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public TipoMovimientoEnum TipoMovimiento { get; set; }
        public decimal Valor { get; set; }

        public decimal Saldo { get; set; }

        public int CuentaId { get; set; }

        public virtual Cuenta Cuenta { get; set; }= null!;
    }

    public enum TipoMovimientoEnum
    {
        Deposito,
        Retiro
    }
}
