using Dapper;
using Microsoft.Data.SqlClient;
using TiendaVentas.Web.Models;

namespace TiendaVentas.Web.Services
{
    public class ProductoService
    {
        private readonly string _connectionString;

        public ProductoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<List<Producto>> ObtenerProductosAsync(int? idCategoria = null)
        {
            var sql = @"
                SELECT
                    P.ID_PRODUCTO  AS Id_Producto,
                    P.ID_CATEGORIA AS Id_Categoria,
                    P.NOMBRE       AS Nombre,
                    P.DESCRIPCION  AS Descripcion,
                    P.PRECIO       AS Precio,
                    P.STOCK        AS Stock,
                    P.IMAGEN_URL   AS Imagen_Url,
                    P.ESTADO       AS Estado,
                    C.NOMBRE       AS Categoria
                FROM PRODUCTOS P
                INNER JOIN CATEGORIAS C ON C.ID_CATEGORIA = P.ID_CATEGORIA
                WHERE P.ESTADO = 'A'";

            if (idCategoria.HasValue && idCategoria.Value > 0)
            {
                sql += " AND P.ID_CATEGORIA = @IdCategoria";
            }

            sql += " ORDER BY P.ID_PRODUCTO DESC";

            using var connection = new SqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<Producto>(sql, new
            {
                IdCategoria = idCategoria
            });

            return resultado.ToList();
        }

        public async Task<List<Producto>> ObtenerProductosDestacadosAsync(int cantidad = 8)
        {
            const string sql = @"
                SELECT TOP (@Cantidad)
                    P.ID_PRODUCTO  AS Id_Producto,
                    P.ID_CATEGORIA AS Id_Categoria,
                    P.NOMBRE       AS Nombre,
                    P.DESCRIPCION  AS Descripcion,
                    P.PRECIO       AS Precio,
                    P.STOCK        AS Stock,
                    P.IMAGEN_URL   AS Imagen_Url,
                    P.ESTADO       AS Estado,
                    C.NOMBRE       AS Categoria
                FROM PRODUCTOS P
                INNER JOIN CATEGORIAS C ON C.ID_CATEGORIA = P.ID_CATEGORIA
                WHERE P.ESTADO = 'A'
                ORDER BY P.ID_PRODUCTO DESC";

            using var connection = new SqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<Producto>(sql, new
            {
                Cantidad = cantidad
            });

            return resultado.ToList();
        }

        public async Task<Producto?> ObtenerProductoPorIdAsync(int id)
        {
            const string sql = @"
                SELECT
                    P.ID_PRODUCTO  AS Id_Producto,
                    P.ID_CATEGORIA AS Id_Categoria,
                    P.NOMBRE       AS Nombre,
                    P.DESCRIPCION  AS Descripcion,
                    P.PRECIO       AS Precio,
                    P.STOCK        AS Stock,
                    P.IMAGEN_URL   AS Imagen_Url,
                    P.ESTADO       AS Estado,
                    C.NOMBRE       AS Categoria
                FROM PRODUCTOS P
                INNER JOIN CATEGORIAS C ON C.ID_CATEGORIA = P.ID_CATEGORIA
                WHERE P.ID_PRODUCTO = @Id";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
        }

        public async Task<List<Producto>> ObtenerTodosAdminAsync()
        {
            const string sql = @"
                SELECT
                    P.ID_PRODUCTO  AS Id_Producto,
                    P.ID_CATEGORIA AS Id_Categoria,
                    P.NOMBRE       AS Nombre,
                    P.DESCRIPCION  AS Descripcion,
                    P.PRECIO       AS Precio,
                    P.STOCK        AS Stock,
                    P.IMAGEN_URL   AS Imagen_Url,
                    P.ESTADO       AS Estado,
                    C.NOMBRE       AS Categoria
                FROM PRODUCTOS P
                INNER JOIN CATEGORIAS C ON C.ID_CATEGORIA = P.ID_CATEGORIA
                ORDER BY P.ID_PRODUCTO DESC";

            using var connection = new SqlConnection(_connectionString);
            var resultado = await connection.QueryAsync<Producto>(sql);
            return resultado.ToList();
        }

        public async Task CrearAsync(Producto model)
        {
            const string sql = @"
                INSERT INTO PRODUCTOS
                (
                    ID_CATEGORIA,
                    NOMBRE,
                    DESCRIPCION,
                    PRECIO,
                    STOCK,
                    IMAGEN_URL,
                    ESTADO,
                    FECHA_CREACION
                )
                VALUES
                (
                    @Id_Categoria,
                    @Nombre,
                    @Descripcion,
                    @Precio,
                    @Stock,
                    @Imagen_Url,
                    @Estado,
                    GETDATE()
                )";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, model);
        }

        public async Task ActualizarAsync(Producto model)
        {
            const string sql = @"
                UPDATE PRODUCTOS
                SET ID_CATEGORIA = @Id_Categoria,
                    NOMBRE = @Nombre,
                    DESCRIPCION = @Descripcion,
                    PRECIO = @Precio,
                    STOCK = @Stock,
                    IMAGEN_URL = @Imagen_Url,
                    ESTADO = @Estado
                WHERE ID_PRODUCTO = @Id_Producto";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, model);
        }

        public async Task BajaLogicaAsync(int id)
        {
            const string sql = @"
                UPDATE PRODUCTOS
                SET ESTADO = 'I'
                WHERE ID_PRODUCTO = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}