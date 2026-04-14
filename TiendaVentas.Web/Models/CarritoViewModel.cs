namespace TiendaVentas.Web.Models
{
    public class CarritoViewModel
    {
        public List<CarritoItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(x => x.Subtotal);
        public int TotalItems => Items.Sum(x => x.Cantidad);
    }
}