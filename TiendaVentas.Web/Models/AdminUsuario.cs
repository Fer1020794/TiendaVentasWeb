namespace TiendaVentas.Web.Models
{
    public class AdminUsuario
    {
        public int Id_Usuario { get; set; }
        public string Nombre_Usuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string Rol { get; set; } = "ADMIN";
        public string Estado { get; set; } = "A";
    }
}