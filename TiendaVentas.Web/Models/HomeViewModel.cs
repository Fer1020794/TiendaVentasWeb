namespace TiendaVentas.Web.Models
{
    public class HomeViewModel
    {
        public List<Categoria> Categorias { get; set; } = new();
        public List<Producto> ProductosDestacados { get; set; } = new();
        public List<Banner> Banners { get; set; } = new();
    }
}