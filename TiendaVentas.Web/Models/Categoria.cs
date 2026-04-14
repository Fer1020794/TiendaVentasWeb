namespace TiendaVentas.Web.Models
{
    public class Categoria
    {
        public int Id_Categoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = "A";
    }
}