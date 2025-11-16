
namespace Sistema_Tickets.ViewModels.Perfil
{
    public class PerfilViewModel
    {
        public int PerfilID { get; set; }
        public string Nombre { get; set; } = null!;

        public string Correo { get; set; } = null!;
        public string? Imagen { get; set; }
        public string RolNombre { get; set; } = null!;
        public string PerfilDepartamento { get; set; } = null!;

        public bool Estado { get; set; }
        public bool bloqueado { get; set; }

        public DateTime? DateTime { get; set; }
        public DateTime? UltimaSesion { get; set; }
    }
}
