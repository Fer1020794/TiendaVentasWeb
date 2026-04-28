using Dapper;
using MySqlConnector;
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

        public async Task<List<Producto>> ObtenerProductosAsync(int? idCategoria = null, string? texto = null)
        {
            const string sql = @"
                SELECT
                    P.ID_PRODUCTO      AS Id_Producto,
                    P.ID_CATEGORIA     AS Id_Categoria,
                    P.CODIGO_PRODUCTO  AS Codigo_Producto,
                    P.NOMBRE           AS Nombre,
                    P.DESCRIPCION      AS Descripcion,
                    P.PRECIO           AS Precio,
                    P.STOCK            AS Stock,
                    P.IMAGEN_URL       AS Imagen_Url,
                    P.ESTADO           AS Estado,
                    C.NOMBRE           AS Categoria
                FROM PRODUCTOS P
                INNER JOIN CATEGORIAS C ON C.ID_CATEGORIA = P.ID_CATEGORIA
                WHERE P.ESTADO = 'A'
                  AND (@IdCategoria IS NULL OR @IdCategoria <= 0 OR P.ID_CATEGORIA = @IdCategoria)
                  AND (
                        @Texto IS NULL OR @Texto = ''
                        OR P.CODIGO_PRODUCTO LIKE CONCAT('%', @Texto, '%')
                        OR P.NOMBRE LIKE CONCAT('%', @Texto, '%')
                        OR P.DESCRIPCION LIKE CONCAT('%', @Texto, '%')
                        OR C.NOMBRE LIKE CONCAT('%', @Texto, '%')
                      )
                ORDER BY P.ID_PRODUCTO DESC;";

            using var connection = new MySqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<Producto>(sql, new
            {
                IdCategoria = idCategoria,
                Texto = texto?.Trim()
            });

            return resultado.ToList();
        }

        public async Task<List<Producto>> ObtenerProductosDestacadosAsync(int cantidad = 8)
        {
            const string sql = @"
                SELECT
                    P.ID_PRODUCTO      AS Id_Producto,
                    P.ID_CATEGORIA     AS Id_Categoria,
                    P.CODIGO_PRODUCTO  AS Codigo_Producto,
                    P.NOMBRE           AS Nombre,
                    P.DESCRIPCION      AS Descripcion,
                    P.PRECIO           AS Precio,
                    P.STOCK            AS Stock,
                    P.IMAGEN_URL       AS Imagen_Url,
                    P.ESTADO           AS Estado,
                    C.NOMBRE           AS Categoria
                FROM PRODUCTOS P
                INNER JOIN CATEGORIAS C ON C.ID_CATEGORIA = P.ID_CATEGORIA
                WHERE P.ESTADO = 'A'
                ORDER BY P.ID_PRODUCTO DESC
                LIMIT @Cantidad;";

            using var connection = new MySqlConnection(_connectionString);

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
                    P.ID_PRODUCTO      AS Id_Producto,
                    P.ID_CATEGORIA     AS Id_Categoria,
                    P.CODIGO_PRODUCTO  AS Codigo_Producto,
                    P.NOMBRE           AS Nombre,
                    P.DESCRIPCION      AS Descripcion,
                    P.PRECIO           AS Precio,
                    P.STOCK            AS Stock,
                    P.IMAGEN_URL       AS Imagen_Url,
                    P.ESTADO           AS Estado,
                    C.NOMBRE           AS Categoria
                FROM PRODUCTOS P
                INNER JOIN CATEGORIAS C ON C.ID_CATEGORIA = P.ID_CATEGORIA
                WHERE P.ID_PRODUCTO = @Id;";

            using var connection = new MySqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
        }

        public async Task<List<Producto>> ObtenerTodosAdminAsync()
        {
            const string sql = @"
                SELECT
                    P.ID_PRODUCTO      AS Id_Producto,
                    P.ID_CATEGORIA     AS Id_Categoria,
                    P.CODIGO_PRODUCTO  AS Codigo_Producto,
                    P.NOMBRE           AS Nombre,
                    P.DESCRIPCION      AS Descripcion,
                    P.PRECIO           AS Precio,
                    P.STOCK            AS Stock,
                    P.IMAGEN_URL       AS Imagen_Url,
                    P.ESTADO           AS Estado,
                    C.NOMBRE           AS Categoria
                FROM PRODUCTOS P
                INNER JOIN CATEGORIAS C ON C.ID_CATEGORIA = P.ID_CATEGORIA
                ORDER BY P.ID_PRODUCTO DESC;";

            using var connection = new MySqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<Producto>(sql);
            return resultado.ToList();
        }

        public async Task CrearAsync(Producto model)
        {
            const string sql = @"
                INSERT INTO PRODUCTOS
                (
                    ID_CATEGORIA,
                    CODIGO_PRODUCTO,
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
                    NULLIF(@Codigo_Producto, ''),
                    @Nombre,
                    @Descripcion,
                    @Precio,
                    @Stock,
                    @Imagen_Url,
                    @Estado,
                    NOW()
                );

                SET @NuevoId = LAST_INSERT_ID();

                UPDATE PRODUCTOS
                SET CODIGO_PRODUCTO = @NuevoId
                WHERE ID_PRODUCTO = @NuevoId
                  AND CODIGO_PRODUCTO IS NULL;";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, new
            {
                model.Id_Categoria,
                Codigo_Producto = model.Codigo_Producto?.Trim(),
                model.Nombre,
                model.Descripcion,
                model.Precio,
                model.Stock,
                model.Imagen_Url,
                model.Estado
            });
        }

        public async Task ActualizarAsync(Producto model)
        {
            const string sql = @"
                UPDATE PRODUCTOS
                SET ID_CATEGORIA = @Id_Categoria,
                    CODIGO_PRODUCTO = NULLIF(@Codigo_Producto, ''),
                    NOMBRE = @Nombre,
                    DESCRIPCION = @Descripcion,
                    PRECIO = @Precio,
                    STOCK = @Stock,
                    IMAGEN_URL = @Imagen_Url,
                    ESTADO = @Estado
                WHERE ID_PRODUCTO = @Id_Producto;

                UPDATE PRODUCTOS
                SET CODIGO_PRODUCTO = ID_PRODUCTO
                WHERE ID_PRODUCTO = @Id_Producto
                  AND CODIGO_PRODUCTO IS NULL;";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, new
            {
                model.Id_Producto,
                model.Id_Categoria,
                Codigo_Producto = model.Codigo_Producto?.Trim(),
                model.Nombre,
                model.Descripcion,
                model.Precio,
                model.Stock,
                model.Imagen_Url,
                model.Estado
            });
        }

        public async Task BajaLogicaAsync(int id)
        {
            const string sql = @"
                UPDATE PRODUCTOS
                SET ESTADO = 'I'
                WHERE ID_PRODUCTO = @Id;";

            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}