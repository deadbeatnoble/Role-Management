using Microsoft.Data.SqlClient;
using System.Data.SqlClient;

namespace RoleManagement.Data
{
    public static class SqlQuery
    {
        private static string ConnectionString { get; set; } = string.Empty;

        public static void Initialize(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static async Task<List<T>> QueryAsync<T>(string sql, Func<SqlDataReader, T> mapFunc) => 
            await QueryAsync(sql, mapFunc, Array.Empty<SqlParameter>());

        public static async Task<List<T>> QueryAsync<T>(string sql, Func<SqlDataReader, T> mapFunc, params SqlParameter[] parameters)
        {
            var result = new List<T>();

            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            if (parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(mapFunc(reader));
            }

            return result;
        }

        public static async Task ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<T> QueryScalarAsync<T>(string sql, params SqlParameter[] parameters)
        {
            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            if (parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            var result = await cmd.ExecuteScalarAsync();
            return (T)result;
        }
    }
}
