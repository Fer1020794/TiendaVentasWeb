namespace TiendaVentas.Web.Models
{
    public class CarritoItem
    {
        public int Id_Producto { get; set; }
        public string? Codigo_Producto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Imagen_Url { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string? Categoria { get; set; }

        public decimal Subtotal => Precio * Cantidad;
    }
}