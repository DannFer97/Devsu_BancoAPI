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

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+]).{6,100}$",
             ErrorMessage = "La contraseña debe tener al menos una mayúscula, un número y un carácter especial")]
        [DataType(DataType.Password)]

        public string Contrasena { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;

        [InverseProperty("Cliente")]
        public virtual ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();

    }
}
