using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TiendaVentas.Web.Models
{
    public class ProductoFormViewModel
    {
        public int Id_Producto { get; set; }

        [Required]
        public int Id_Categoria { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; }

        public string? Imagen_Url { get; set; }

        public IFormFile? ImagenFile { get; set; }

        [Required]
        public string Estado { get; set; } = "A";
    }
}