using System.ComponentModel.DataAnnotations;

namespace Sistema_Tickets.ViewModels.Perfil
{
    public class CambiarImagenViewModel
    {
        public int PerfilID { get; set; }

        [Display(Name = "Nueva Foto de Perfil")]
        [DataType(DataType.Upload)]
        public IFormFile? Imagen { get; set; }

        
    }
}
