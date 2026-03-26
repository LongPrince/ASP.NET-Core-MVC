using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Partner;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho Khách hàng trên SQL Server
    /// Kế thừa và triển khai các phương thức từ ICustomerRepository.
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo Repository với chuỗi kết nối
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến SQL Server.</param>
        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới một khách hàng
        /// </summary>
        public async Task<int> AddAsync(Customer data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Customers(CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
                               VALUES(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Password, @IsLocked);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.ExecuteScalarAsync<int>(sql, data);
            }
        }

        /// <summary>
        /// Xóa khách hàng theo ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Customers WHERE CustomerID = @id";
                int rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo mã ID
        /// </summary>
        public async Task<Customer?> GetAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Customers WHERE CustomerID = @id";
                return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { id });
            }
        }

        /// <summary>
        /// Kiểm tra xem khách hàng có đang được sử dụng ở bảng khác không, ví dụ trong bảng Đơn hàng (Orders)
        /// </summary>
        public async Task<bool> IsUsedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Giả sử bảng Orders tham chiếu đến CustomerID
                string sql = "SELECT CASE WHEN EXISTS(SELECT 1 FROM Orders WHERE CustomerID = @id) THEN 1 ELSE 0 END";
                return await connection.ExecuteScalarAsync<bool>(sql, new { id });
            }
        }

        /// <summary>
        /// Tìm kiếm, lấy dữ liệu khách hàng dưới dạng phân trang.
        /// </summary>
        public async Task<PagedResult<Customer>> ListAsync(PaginationSearchInput input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Customers 
                               WHERE (@searchValue = N'') OR (CustomerName LIKE @searchValue) OR (ContactName LIKE @searchValue)
                               ORDER BY CustomerID
                               OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                               
                               SELECT COUNT(*) FROM Customers 
                               WHERE (@searchValue = N'') OR (CustomerName LIKE @searchValue) OR (ContactName LIKE @searchValue);";

                var parameters = new
                {
                    searchValue = string.IsNullOrEmpty(input.SearchValue) ? "" : $"%{input.SearchValue}%",
                    offset = (input.Page - 1) * input.PageSize,
                    pageSize = input.PageSize
                };

                using (var multi = await connection.QueryMultipleAsync(sql, parameters))
                {
                    var data = (await multi.ReadAsync<Customer>()).ToList();
                    var rowCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<Customer> 
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
        /// Cập nhật thông tin khách hàng
        /// </summary>
        public async Task<bool> UpdateAsync(Customer data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Customers 
                               SET CustomerName = @CustomerName, 
                                   ContactName = @ContactName, 
                                   Province = @Province, 
                                   Address = @Address, 
                                   Phone = @Phone, 
                                   Email = @Email, 
                                   IsLocked = @IsLocked
                               WHERE CustomerID = @CustomerID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Kiểm tra xem email có tồn tại chưa (hỗ trợ trường hợp thêm mới id=0 và cập nhật id!=0)
        /// </summary>
        public async Task<bool> ValidateEmailAsync(string email, int id = 0)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT CASE WHEN EXISTS(
                                   SELECT 1 FROM Customers WHERE Email = @email AND CustomerID <> @id
                               ) THEN 1 ELSE 0 END";
                return await connection.ExecuteScalarAsync<bool>(sql, new { email, id });
            }
        }
    }
}
