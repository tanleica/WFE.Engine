using Npgsql;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using MySql.Data.MySqlClient;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.Persistence
{
    public static class DbConnectionHelper
    {
        public static async Task UseOpenConnectionAsync(
            string dbType,
            string connectionString,
            Func<DbConnection, Task> action)
        {
            DbConnection connection = dbType switch
            {
                DbTypes.SqlServer => new SqlConnection(connectionString),
                DbTypes.Oracle => new OracleConnection(connectionString),
                DbTypes.Postgres => new NpgsqlConnection(connectionString),
                DbTypes.MySql      => new MySqlConnection(connectionString),
                _ => throw new NotSupportedException($"Unsupported DB type: {dbType}")
            };

            await using (connection)
            {
                await connection.OpenAsync();
                await action(connection);
            }
        }
    }
}