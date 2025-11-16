using System.ComponentModel.DataAnnotations;

namespace Sistema_Tickets.ViewModels.Usuarios
{
    public class UsuarioEditViewModel
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(150)]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        // Contraseña OPCIONAL en edición
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        [Display(Name = "Departamento")]
        public int? DepartamentoId { get; set; }

        [Display(Name = "Usuario Activo")]
        public bool Estado { get; set; }

        [Display(Name = "Usuario Bloqueado")]
        public bool Esbloqueado { get; set; }

        [Display(Name = "Nueva Foto de Perfil")]
        [DataType(DataType.Upload)]
        public IFormFile? ImagenArchivo { get; set; }

        // Para mostrar la imagen actual
        public string? ImagenActual { get; set; }

        [Display(Name = "Eliminar Foto Actual")]
        public bool EliminarImagen { get; set; }
    }
}
