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
    /// Entidad que representa a un cliente
    /// </summary>
    public class Cliente : Persona
    {


        public string Contrasena { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;

        public virtual ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();

    }
}
