using Dapper;
using MySqlConnector;
using TiendaVentas.Web.Models;

namespace TiendaVentas.Web.Services
{
    public class PedidoService
    {
        private readonly string _connectionString;

        public PedidoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<int> CrearPedidoAsync(Pedido pedido, List<CarritoItem> items)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string sqlPedido = @"
                    INSERT INTO PEDIDOS
                    (
                        NOMBRE_CLIENTE,
                        TELEFONO,
                        CORREO_CLIENTE,
                        DIRECCION,
                        DEPARTAMENTO,
                        MUNICIPIO,
                        OBSERVACIONES,
                        TOTAL,
                        ESTADO,
                        FECHA_CREACION
                    )
                    VALUES
                    (
                        @Nombre_Cliente,
                        @Telefono,
                        @Correo_Cliente,
                        @Direccion,
                        @Departamento,
                        @Municipio,
                        @Observaciones,
                        @Total,
                        @Estado,
                        NOW()
                    );

                    SELECT LAST_INSERT_ID();";

                var idPedido = await connection.ExecuteScalarAsync<int>(
                    sqlPedido,
                    new
                    {
                        pedido.Nombre_Cliente,
                        pedido.Telefono,
                        pedido.Correo_Cliente,
                        pedido.Direccion,
                        pedido.Departamento,
                        pedido.Municipio,
                        pedido.Observaciones,
                        pedido.Total,
                        pedido.Estado
                    },
                    transaction
                );

                const string sqlDetalle = @"
                    INSERT INTO PEDIDO_DETALLE
                    (
                        ID_PEDIDO,
                        ID_PRODUCTO,
                        NOMBRE_PRODUCTO,
                        CANTIDAD,
                        PRECIO_UNITARIO,
                        SUBTOTAL
                    )
                    VALUES
                    (
                        @Id_Pedido,
                        @Id_Producto,
                        @Nombre_Producto,
                        @Cantidad,
                        @Precio_Unitario,
                        @Subtotal
                    );";

                foreach (var item in items)
                {
                    await connection.ExecuteAsync(
                        sqlDetalle,
                        new
                        {
                            Id_Pedido = idPedido,
                            Id_Producto = item.Id_Producto,
                            Nombre_Producto = item.Nombre,
                            Cantidad = item.Cantidad,
                            Precio_Unitario = item.Precio,
                            Subtotal = item.Subtotal
                        },
                        transaction
                    );
                }

                await transaction.CommitAsync();
                return idPedido;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("ERROR CREAR PEDIDO: " + ex.Message);
                throw;
            }
        }

        public async Task ActualizarPdfUrlAsync(int idPedido, string pdfUrl)
        {
            const string sql = @"
                UPDATE PEDIDOS
                SET PDF_URL = @PdfUrl
                WHERE ID_PEDIDO = @IdPedido;";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, new
            {
                PdfUrl = pdfUrl,
                IdPedido = idPedido
            });
        }

        public async Task<List<Pedido>> ObtenerPedidosAsync()
        {
            const string sql = @"
                SELECT
                    ID_PEDIDO       AS Id_Pedido,
                    NOMBRE_CLIENTE  AS Nombre_Cliente,
                    TELEFONO        AS Telefono,
                    CORREO_CLIENTE  AS Correo_Cliente,
                    DIRECCION       AS Direccion,
                    DEPARTAMENTO    AS Departamento,
                    MUNICIPIO       AS Municipio,
                    OBSERVACIONES   AS Observaciones,
                    TOTAL           AS Total,
                    ESTADO          AS Estado,
                    PDF_URL         AS Pdf_Url,
                    FECHA_CREACION  AS Fecha_Creacion
                FROM PEDIDOS
                ORDER BY ID_PEDIDO DESC;";

            using var connection = new MySqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<Pedido>(sql);
            return resultado.ToList();
        }

        public async Task<Pedido?> ObtenerPedidoPorIdAsync(int idPedido)
        {
            const string sql = @"
                SELECT
                    ID_PEDIDO       AS Id_Pedido,
                    NOMBRE_CLIENTE  AS Nombre_Cliente,
                    TELEFONO        AS Telefono,
                    CORREO_CLIENTE  AS Correo_Cliente,
                    DIRECCION       AS Direccion,
                    DEPARTAMENTO    AS Departamento,
                    MUNICIPIO       AS Municipio,
                    OBSERVACIONES   AS Observaciones,
                    TOTAL           AS Total,
                    ESTADO          AS Estado,
                    PDF_URL         AS Pdf_Url,
                    FECHA_CREACION  AS Fecha_Creacion
                FROM PEDIDOS
                WHERE ID_PEDIDO = @IdPedido;";

            using var connection = new MySqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<Pedido>(sql, new { IdPedido = idPedido });
        }

        public async Task<List<PedidoDetalle>> ObtenerDetallePedidoAsync(int idPedido)
        {
            const string sql = @"
                SELECT
                    ID_DETALLE      AS Id_Detalle,
                    ID_PEDIDO       AS Id_Pedido,
                    ID_PRODUCTO     AS Id_Producto,
                    NOMBRE_PRODUCTO AS Nombre_Producto,
                    CANTIDAD        AS Cantidad,
                    PRECIO_UNITARIO AS Precio_Unitario,
                    SUBTOTAL        AS Subtotal
                FROM PEDIDO_DETALLE
                WHERE ID_PEDIDO = @IdPedido
                ORDER BY ID_DETALLE;";

            using var connection = new MySqlConnection(_connectionString);

            var resultado = await connection.QueryAsync<PedidoDetalle>(sql, new { IdPedido = idPedido });
            return resultado.ToList();
        }

        public async Task ActualizarEstadoAsync(int idPedido, string estado)
        {
            const string sql = @"
                UPDATE PEDIDOS
                SET ESTADO = @Estado
                WHERE ID_PEDIDO = @IdPedido;";

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(sql, new
            {
                Estado = estado,
                IdPedido = idPedido
            });
        }
    }
}