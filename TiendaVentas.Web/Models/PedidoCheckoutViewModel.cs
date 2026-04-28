using System.ComponentModel.DataAnnotations;

namespace TiendaVentas.Web.Models
{
    public class PedidoCheckoutViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre_Cliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Correo inválido.")]
        public string? Correo_Cliente { get; set; }

        [Required(ErrorMessage = "La dirección o referencia es obligatoria.")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El departamento es obligatorio.")]
        public string Departamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El municipio es obligatorio.")]
        public string Municipio { get; set; } = string.Empty;

        public string? Observaciones { get; set; }

        public List<CarritoItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(x => x.Subtotal);
    }
}