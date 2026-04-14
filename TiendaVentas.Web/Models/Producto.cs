namespace TiendaVentas.Web.Models
{
    public class Producto
    {
        public int Id_Producto { get; set; }
        public int Id_Categoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? Imagen_Url { get; set; }
        public string Estado { get; set; } = "A";
        public string? Categoria { get; set; }
    }
}