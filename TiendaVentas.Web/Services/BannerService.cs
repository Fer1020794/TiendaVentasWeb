using Dapper;
using MySqlConnector;
using TiendaVentas.Web.Models;

namespace TiendaVentas.Web.Services
{
    public class BannerService
    {
        private readonly string _connectionString;

        public BannerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        
        public async Task<List<Banner>> ObtenerActivosAsync()
        {
            const string sql = @"
                SELECT
                    ID_BANNER AS Id_Banner,
                    TITULO AS Titulo,
                    SUBTITULO AS Subtitulo,
                    IMAGEN_URL AS Imagen_Url,
                    BOTON_TEXTO AS Boton_Texto,
                    BOTON_URL AS Boton_Url,
                    ORDEN AS Orden,
                    ESTADO AS Estado,
                    FECHA_CREACION AS Fecha_Creacion
                FROM BANNERS
                WHERE ESTADO = 'A'
                ORDER BY ORDEN ASC, ID_BANNER DESC;";

            using var connection = new MySqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<Banner>(sql);
            return resultado.ToList();
        }

        
        public async Task<List<Banner>> ObtenerTodosAdminAsync()
        {
            const string sql = @"
                SELECT
                    ID_BANNER AS Id_Banner,
                    TITULO AS Titulo,
                    SUBTITULO AS Subtitulo,
                    IMAGEN_URL AS Imagen_Url,
                    BOTON_TEXTO AS Boton_Texto,
                    BOTON_URL AS Boton_Url,
                    ORDEN AS Orden,
                    ESTADO AS Estado,
                    FECHA_CREACION AS Fecha_Creacion
                FROM BANNERS
                ORDER BY ORDEN ASC, ID_BANNER DESC;";

            using var connection = new MySqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<Banner>(sql);
            return resultado.ToList();
        }

        
        public async Task<Banner?> ObtenerPorIdAsync(int id)
        {
            const string sql = @"
                SELECT
                    ID_BANNER AS Id_Banner,
                    TITULO AS Titulo,
                    SUBTITULO AS Subtitulo,
                    IMAGEN_URL AS Imagen_Url,
                    BOTON_TEXTO AS Boton_Texto,
                    BOTON_URL AS Boton_Url,
                    ORDEN AS Orden,
                    ESTADO AS Estado,
                    FECHA_CREACION AS Fecha_Creacion
                FROM BANNERS
                WHERE ID_BANNER = @Id;";

            using var connection = new MySqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<Banner>(sql, new { Id = id });
        }

        
        public async Task CrearAsync(Banner model)
        {
            const string sql = @"
                INSERT INTO BANNERS
                (
                    TITULO,
                    SUBTITULO,
                    IMAGEN_URL,
                    BOTON_TEXTO,
                    BOTON_URL,
                    ORDEN,
                    ESTADO,
                    FECHA_CREACION
                )
                VALUES
                (
                    @Titulo,
                    @Subtitulo,
                    @Imagen_Url,
                    @Boton_Texto,
                    @Boton_Url,
                    @Orden,
                    @Estado,
                    NOW()
                );";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, model);
        }

     
        public async Task ActualizarAsync(Banner model)
        {
            const string sql = @"
                UPDATE BANNERS
                SET TITULO = @Titulo,
                    SUBTITULO = @Subtitulo,
                    IMAGEN_URL = @Imagen_Url,
                    BOTON_TEXTO = @Boton_Texto,
                    BOTON_URL = @Boton_Url,
                    ORDEN = @Orden,
                    ESTADO = @Estado
                WHERE ID_BANNER = @Id_Banner;";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, model);
        }

        
        public async Task BajaLogicaAsync(int id)
        {
            const string sql = @"
                UPDATE BANNERS
                SET ESTADO = 'I'
                WHERE ID_BANNER = @Id;";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        
        public async Task ActivarAsync(int id)
        {
            const string sql = @"
                UPDATE BANNERS
                SET ESTADO = 'A'
                WHERE ID_BANNER = @Id;";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}