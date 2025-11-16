using System.ComponentModel.DataAnnotations;

namespace Sistema_Tickets.ViewModels.Perfil
{
    public class CambiarContraseñaViewModel
    {
        public int PerfilID { get; set; }

        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string PasswordActual { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos {2} caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula y un número")]
        public string PasswordNueva { get; set; }

        [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("PasswordNueva", ErrorMessage = "Las contraseñas no coinciden")]
        public string PasswordConfirmar { get; set; }
    }
}
