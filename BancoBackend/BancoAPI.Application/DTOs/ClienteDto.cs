using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BancoAPI.Application.DTOs
{
    /// <summary>
    /// DTO para crear un nuevo cliente
    /// </summary>
    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(10)]
        public string Genero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(18, 120)]
        public int Edad { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [RegularExpression(@"^\d{1,20}$", ErrorMessage = "La identificación debe tener solo numeros y estar entre 1 y 20 caractéres")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^\d{7,15}$", ErrorMessage = "El teléfono solo debe contener números y tener entre 7 y 15 dígitos")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+]).{6,100}$",
              ErrorMessage = "La contraseña debe tener al menos una mayúscula, un número y un carácter especial")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;
    }

    /// <summary>
    /// DTO para actualizar un cliente existente
    /// </summary>
    public class ClienteUpdateDto
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string? Nombre { get; set; }

        [StringLength(10, MinimumLength = 1, ErrorMessage = "El género debe tener entre 1 y 10 caracteres")]
        public string? Genero { get; set; }

        [Range(18, 120, ErrorMessage = "La edad debe estar entre 18 y 120 años")]
        public int? Edad { get; set; }

        [StringLength(200, MinimumLength = 1, ErrorMessage = "La dirección debe tener entre 1 y 200 caracteres")]
        public string? Direccion { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [RegularExpression(@"^\d{7,15}$", ErrorMessage = "El teléfono solo debe contener números y tener entre 7 y 15 dígitos")]
        public string? Telefono { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+]).{6,100}$",
            ErrorMessage = "La contraseña debe tener al menos una mayúscula, un número y un carácter especial")]
        public string? Contrasena { get; set; }

        public bool? Estado { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para Cliente
    /// </summary>
    public class ClienteDto
    {
        public int ClienteId { get; set; }  // PersonaId mapeado como ClienteId para compatibilidad
        public string Nombre { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public bool Estado { get; set; }
        public List<CuentaDto>? Cuentas { get; set; }
    }
}
