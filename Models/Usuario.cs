using System.ComponentModel.DataAnnotations;

namespace mvc.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [RegularExpression(
            @"^[a-zA-Z0-9_]+$",
            ErrorMessage = "El nombre de usuario solo puede contener letras, números y guiones bajos."
        )]
        public string NombreUsuario { get; set; } = "";

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(
            @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$",
            ErrorMessage = "El nombre solo puede contener letras y espacios."
        )]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [RegularExpression(
            @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$",
            ErrorMessage = "El apellido solo puede contener letras y espacios."
        )]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un email válido.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = "";

        public string? Avatar { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        [RegularExpression(
            "^(ADMINISTRADOR|EMPLEADO)$",
            ErrorMessage = "El rol seleccionado no es válido."
        )]
        public string Rol { get; set; } = "";
    }
}