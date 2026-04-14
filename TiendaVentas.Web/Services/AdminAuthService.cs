using Dapper;
using Microsoft.Data.SqlClient;
using TiendaVentas.Web.Models;

namespace TiendaVentas.Web.Services
{
    public class AdminAuthService
    {
        private readonly string _connectionString;

        public AdminAuthService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<AdminUsuario?> LoginAsync(string correo, string clave)
        {
            const string sql = @"
                SELECT
                    ID_USUARIO      AS Id_Usuario,
                    NOMBRE_USUARIO  AS Nombre_Usuario,
                    CORREO          AS Correo,
                    CLAVE           AS Clave,
                    ROL             AS Rol,
                    ESTADO          AS Estado
                FROM ADMIN_USUARIOS
                WHERE UPPER(CORREO) = UPPER(@Correo)
                  AND CLAVE = @Clave
                  AND ESTADO = 'A'";

            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<AdminUsuario>(sql, new
            {
                Correo = correo,
                Clave = clave
            });
        }
    }
}