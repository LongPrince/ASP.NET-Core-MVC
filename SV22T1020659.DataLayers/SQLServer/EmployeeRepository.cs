using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.HR;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho Nhân viên (Employee) trên SQL Server
    /// Kế thừa và triển khai các phương thức từ IEmployeeRepository
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo Repository với chuỗi kết nối
        /// </summary>
        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới một bản ghi nhân viên vào cơ sở dữ liệu.
        /// </summary>
        public async Task<int> AddAsync(Employee data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Employees(FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking, RoleNames)
                               VALUES(@FullName, @BirthDate, @Address, @Phone, @Email, @Password, @Photo, @IsWorking, @RoleNames);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.ExecuteScalarAsync<int>(sql, data);
            }
        }

        /// <summary>
        /// Xóa một nhân viên khỏi cơ sở dữ liệu dựa vào mã ID.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Employees WHERE EmployeeID = @id";
                int rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Lấy chi tiết thông tin của nhân viên theo mã ID.
        /// </summary>
        public async Task<Employee?> GetAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Employees WHERE EmployeeID = @id";
                return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { id });
            }
        }

        /// <summary>
        /// Kiểm tra xem nhân viên có đang được sử dụng ở bảng khác không (ví dụ trong bảng Đơn hàng).
        /// </summary>
        public async Task<bool> IsUsedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT CASE WHEN EXISTS(SELECT 1 FROM Orders WHERE EmployeeID = @id) THEN 1 ELSE 0 END";
                return await connection.ExecuteScalarAsync<bool>(sql, new { id });
            }
        }

        /// <summary>
        /// Tìm kiếm, lấy dữ liệu nhân viên dưới dạng phân trang.
        /// </summary>
        public async Task<PagedResult<Employee>> ListAsync(PaginationSearchInput input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Employees 
                               WHERE (@searchValue = N'') OR (FullName LIKE @searchValue) OR (Email LIKE @searchValue)
                               ORDER BY EmployeeID
                               OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                               
                               SELECT COUNT(*) FROM Employees 
                               WHERE (@searchValue = N'') OR (FullName LIKE @searchValue) OR (Email LIKE @searchValue);";

                var parameters = new
                {
                    searchValue = string.IsNullOrEmpty(input.SearchValue) ? "" : $"%{input.SearchValue}%",
                    offset = (input.Page - 1) * input.PageSize,
                    pageSize = input.PageSize
                };

                using (var multi = await connection.QueryMultipleAsync(sql, parameters))
                {
                    var data = (await multi.ReadAsync<Employee>()).ToList();
                    var rowCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<Employee> 
                    { 
                        DataItems = data, 
                        RowCount = rowCount, 
                        Page = input.Page, 
                        PageSize = input.PageSize 
                    };
                }
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một nhân viên đã có sẵn trong CSDL.
        /// </summary>
        public async Task<bool> UpdateAsync(Employee data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Employees 
                               SET FullName = @FullName, 
                                   BirthDate = @BirthDate, 
                                   Address = @Address, 
                                   Phone = @Phone, 
                                   Email = @Email, 
                                   Photo = @Photo, 
                                   IsWorking = @IsWorking,
                                   RoleNames = @RoleNames
                               WHERE EmployeeID = @EmployeeID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Kiểm tra email có trùng với nhân viên khác khi thêm/sửa hay không.
        /// </summary>
        public async Task<bool> ValidateEmailAsync(string email, int id = 0)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT CASE WHEN EXISTS(
                                   SELECT 1 FROM Employees WHERE Email = @email AND EmployeeID <> @id
                               ) THEN 1 ELSE 0 END";
                return await connection.ExecuteScalarAsync<bool>(sql, new { email, id });
            }
        }
    }
}
