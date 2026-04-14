namespace TiendaVentas.Web.Models
{
    public class PedidoDetalle
    {
        public int Id_Detalle { get; set; }
        public int Id_Pedido { get; set; }
        public int Id_Producto { get; set; }
        public string Nombre_Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Precio_Unitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}