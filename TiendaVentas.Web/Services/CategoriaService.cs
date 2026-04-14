using Dapper;
using Microsoft.Data.SqlClient;
using TiendaVentas.Web.Models;

namespace TiendaVentas.Web.Services
{
    public class CategoriaService
    {
        private readonly string _connectionString;

        public CategoriaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            const string sql = @"
        ;WITH CategoriasUnicas AS
        (
            SELECT
                ID_CATEGORIA,
                NOMBRE,
                DESCRIPCION,
                ESTADO,
                ROW_NUMBER() OVER (PARTITION BY NOMBRE ORDER BY ID_CATEGORIA) AS RN
            FROM CATEGORIAS
            WHERE ESTADO = 'A'
        )
        SELECT
            ID_CATEGORIA AS Id_Categoria,
            NOMBRE AS Nombre,
            DESCRIPCION AS Descripcion,
            ESTADO AS Estado
        FROM CategoriasUnicas
        WHERE RN = 1
        ORDER BY NOMBRE;";

            using var connection = new SqlConnection(_connectionString);
            var resultado = await connection.QueryAsync<Categoria>(sql);
            return resultado.ToList();
        }

        public async Task<List<Categoria>> ObtenerTodasAdminAsync()
        {
            const string sql = @"
                SELECT
                    ID_CATEGORIA AS Id_Categoria,
                    NOMBRE AS Nombre,
                    DESCRIPCION AS Descripcion,
                    ESTADO AS Estado
                FROM CATEGORIAS
                ORDER BY ID_CATEGORIA DESC";

            using var connection = new SqlConnection(_connectionString);
            var resultado = await connection.QueryAsync<Categoria>(sql);
            return resultado.ToList();
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            const string sql = @"
                SELECT
                    ID_CATEGORIA AS Id_Categoria,
                    NOMBRE AS Nombre,
                    DESCRIPCION AS Descripcion,
                    ESTADO AS Estado
                FROM CATEGORIAS
                WHERE ID_CATEGORIA = @Id";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(sql, new { Id = id });
        }

        public async Task CrearAsync(Categoria model)
        {
            const string sql = @"
                INSERT INTO CATEGORIAS (NOMBRE, DESCRIPCION, ESTADO)
                VALUES (@Nombre, @Descripcion, @Estado)";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, model);
        }

        public async Task ActualizarAsync(Categoria model)
        {
            const string sql = @"
                UPDATE CATEGORIAS
                SET NOMBRE = @Nombre,
                    DESCRIPCION = @Descripcion,
                    ESTADO = @Estado
                WHERE ID_CATEGORIA = @Id_Categoria";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, model);
        }

        public async Task BajaLogicaAsync(int id)
        {
            const string sql = @"
                UPDATE CATEGORIAS
                SET ESTADO = 'I'
                WHERE ID_CATEGORIA = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}