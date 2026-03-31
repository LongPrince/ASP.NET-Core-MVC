using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Security;
using Microsoft.Data.SqlClient;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Xử lý dữ liệu tài khoản khách hàng (Customer) trên SQL Server
    /// </summary>
    public class CustomerAccountRepository : IUserAccountRepository
    {
        private readonly string _connectionString;

        public CustomerAccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserAccount?> AuthenticateAsync(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT CustomerID AS UserId, 
                                      Email AS UserName, 
                                      CustomerName AS DisplayName, 
                                      Email AS Email, 
                                      '' AS Photo, 
                                      '' AS RoleNames
                               FROM Customers 
                               WHERE Email = @userName AND Password = @password AND IsLocked = 0";
                return await connection.QueryFirstOrDefaultAsync<UserAccount>(sql, new { userName, password });
            }
        }

        public async Task<bool> ChangePasswordAsync(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Customers SET Password = @password WHERE Email = @userName";
                int rowsAffected = await connection.ExecuteAsync(sql, new { userName, password });
                return rowsAffected > 0;
            }
        }
    }
}
