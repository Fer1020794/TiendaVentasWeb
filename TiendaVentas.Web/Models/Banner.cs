namespace TiendaVentas.Web.Models
{
    public class Banner
    {
        public int Id_Banner { get; set; }
        public string? Titulo { get; set; }
        public string? Subtitulo { get; set; }
        public string Imagen_Url { get; set; } = string.Empty;
        public string? Boton_Texto { get; set; }
        public string? Boton_Url { get; set; }
        public int Orden { get; set; } = 1;
        public string Estado { get; set; } = "A";
        public DateTime Fecha_Creacion { get; set; }
    }
}