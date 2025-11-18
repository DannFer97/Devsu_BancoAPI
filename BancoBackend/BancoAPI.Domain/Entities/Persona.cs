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
    /// Entidad base que representa a una persona en el sistema
    /// </summary>
    public class Persona
    {
        public int PersonaId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Genero { get; set; } = string.Empty;

        public int Edad { get; set;}

        public string Identificacion { get; set; }= string.Empty;

        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;


    }
}
