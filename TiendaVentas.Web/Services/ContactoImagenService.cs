using Dapper;
using MySqlConnector;
using TiendaVentas.Web.Models;

namespace TiendaVentas.Web.Services
{
    public class ContactoImagenService
    {
        private readonly string _connectionString;

        public ContactoImagenService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<List<ContactoImagen>> ObtenerTodasAdminAsync()
        {
            const string sql = @"
                SELECT
                    ID_IMAGEN      AS Id_Imagen,
                    TITULO         AS Titulo,
                    IMAGEN_URL     AS Imagen_Url,
                    ESTADO         AS Estado,
                    FECHA_CREACION AS Fecha_Creacion
                FROM CONTACTO_IMAGENES
                ORDER BY ID_IMAGEN DESC;";

            using var connection = new MySqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<ContactoImagen>(sql);
            return resultado.ToList();
        }

        public async Task<List<ContactoImagen>> ObtenerActivasAsync()
        {
            const string sql = @"
                SELECT
                    ID_IMAGEN      AS Id_Imagen,
                    TITULO         AS Titulo,
                    IMAGEN_URL     AS Imagen_Url,
                    ESTADO         AS Estado,
                    FECHA_CREACION AS Fecha_Creacion
                FROM CONTACTO_IMAGENES
                WHERE ESTADO = 'A'
                ORDER BY ID_IMAGEN DESC;";

            using var connection = new MySqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<ContactoImagen>(sql);
            return resultado.ToList();
        }

        public async Task<ContactoImagen?> ObtenerPorIdAsync(int id)
        {
            const string sql = @"
                SELECT
                    ID_IMAGEN      AS Id_Imagen,
                    TITULO         AS Titulo,
                    IMAGEN_URL     AS Imagen_Url,
                    ESTADO         AS Estado,
                    FECHA_CREACION AS Fecha_Creacion
                FROM CONTACTO_IMAGENES
                WHERE ID_IMAGEN = @Id;";

            using var connection = new MySqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<ContactoImagen>(sql, new { Id = id });
        }

        public async Task CrearAsync(ContactoImagen model)
        {
            const string sql = @"
                INSERT INTO CONTACTO_IMAGENES
                (
                    TITULO,
                    IMAGEN_URL,
                    ESTADO,
                    FECHA_CREACION
                )
                VALUES
                (
                    @Titulo,
                    @Imagen_Url,
                    @Estado,
                    NOW()
                );";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, new
            {
                model.Titulo,
                model.Imagen_Url,
                model.Estado
            });
        }

        public async Task CambiarEstadoAsync(int id, string estado)
        {
            const string sql = @"
                UPDATE CONTACTO_IMAGENES
                SET ESTADO = @Estado
                WHERE ID_IMAGEN = @Id;";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Estado = estado
            });
        }

        public async Task DesactivarAsync(int id)
        {
            await CambiarEstadoAsync(id, "I");
        }

        public async Task ActivarAsync(int id)
        {
            await CambiarEstadoAsync(id, "A");
        }
    }
}