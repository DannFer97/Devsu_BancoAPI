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
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonaId { get; set; }

        
        [Required(ErrorMessage ="El nombre es obligatorio")]
        [StringLength(100,MinimumLength =3, ErrorMessage ="El nombre debe tener entre 3 y 100 caractéres")]
        public string Nombre { get; set; } = string.Empty;

        
        [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "El género debe tener entre 3 y 10 caractéres")]
        public string Genero { get; set; } = string.Empty;

        
        [Required(ErrorMessage ="La edad es obligatoria")]
        [Range(18,120, ErrorMessage ="La edad debe estar entre 18 y 120 años")]
        public int Edad { get; set;}

        
        [Required(ErrorMessage = "La identificación es obligatoria")]
        [RegularExpression(@"^\d{1,20}$", ErrorMessage = "La identificación debe tener solo numeros y estar entre 1 y 20 caractéres")]
        public string Identificacion { get; set; }= string.Empty;

        
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;


        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^\d{7,15}$", ErrorMessage = "El teléfono solo debe contener números y tener entre 7 y 15 dígitos")]
        public string Telefono { get; set; } = string.Empty;


    }
}
