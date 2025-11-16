using System.ComponentModel.DataAnnotations;
namespace Sistema_Tickets.ViewModels.Departamentos
{
    public class CrearDepartamentoViewModel
    {
        [Required(ErrorMessage = "El nombre del departamento es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre del Departamento")]
        public string Nombre { get; set; } = null!;
        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres.")]
        [Display(Name = "Descripción del Departamento")]
        public string? Descripcion { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres.")]
        [Display(Name = "Correo Electrónico del Departamento")]
        public string? Email { get; set; }
        public bool Estatus { get; set; } =true;
    }
}
