using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Security;
using Microsoft.Data.SqlClient;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Xử lý dữ liệu tài khoản nhân viên (Employee) trên SQL Server
    /// </summary>
    public class EmployeeAccountRepository : IUserAccountRepository
    {
        private readonly string _connectionString;

        public EmployeeAccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserAccount?> AuthorizeAsync(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Giả định bảng Employees dùng Email làm tên đăng nhập và có 1 trường Password bị ẩn
                // Lưu ý bổ sung trường Password bên SQL Server của bạn nếu có.
                string sql = @"SELECT EmployeeID AS UserId, 
                                      Email AS UserName, 
                                      FullName AS DisplayName, 
                                      Email AS Email, 
                                      Photo, 
                                      RoleNames
                               FROM Employees 
                               WHERE Email = @userName AND Password = @password AND IsWorking = 1";
                return await connection.QueryFirstOrDefaultAsync<UserAccount>(sql, new { userName, password });
            }
        }

        public async Task<bool> ChangePasswordAsync(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Employees SET Password = @password WHERE Email = @userName";
                int rowsAffected = await connection.ExecuteAsync(sql, new { userName, password });
                return rowsAffected > 0;
            }
        }
    }
}
