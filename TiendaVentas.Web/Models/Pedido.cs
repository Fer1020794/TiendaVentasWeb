namespace TiendaVentas.Web.Models
{
    public class Pedido
    {
        public int Id_Pedido { get; set; }
        public string Nombre_Cliente { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Correo_Cliente { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "GENERADO";
        public string? Pdf_Url { get; set; }
        public DateTime Fecha_Creacion { get; set; }
    }
}