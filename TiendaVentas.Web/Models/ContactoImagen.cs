namespace TiendaVentas.Web.Models
{
    public class ContactoImagen
    {
        public int Id_Imagen { get; set; }

        public string? Titulo { get; set; }

        public string Imagen_Url { get; set; } = string.Empty;

        public string Estado { get; set; } = "A";

        public DateTime Fecha_Creacion { get; set; }
    }
}