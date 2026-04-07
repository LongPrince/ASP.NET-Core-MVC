using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Security;
using Microsoft.Data.SqlClient;
using SV22T1020659.Models.Partner;

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

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới
        /// </summary>
        public async Task<int> RegisterCustomerAsync(Customer data, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"IF NOT EXISTS (SELECT * FROM Customers WHERE Email = @Email)
                               BEGIN
                                   INSERT INTO Customers(CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
                                   VALUES(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Password, 0);
                                   SELECT CAST(SCOPE_IDENTITY() AS INT);
                               END
                               ELSE
                                   SELECT -1;";
                return await connection.ExecuteScalarAsync<int>(sql, new
                {
                    data.CustomerName,
                    data.ContactName,
                    data.Province,
                    data.Address,
                    data.Phone,
                    data.Email,
                    Password = password
                });
            }
        }
    }
}
