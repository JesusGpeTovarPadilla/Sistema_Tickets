using System.ComponentModel.DataAnnotations;

namespace Sistema_Tickets.ViewModels.Usuarios
{
    public class UsuarioCreateViewmodel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(150)]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula y un número")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        [Display(Name = "Departamento")]
        public int? DepartamentoId { get; set; }

        [Display(Name = "Usuario Activo")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Foto de Perfil")]
        [DataType(DataType.Upload)]
        public IFormFile? ImagenArchivo { get; set; }

        // Propiedades auxiliares para la vista (no se guardan en BD)
        public string? MensajeAyuda { get; set; }
    }
}
