using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Partner;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho Nhà cung cấp trên SQL Server
    /// Kế thừa và triển khai các phương thức từ IGenericRepository.
    /// </summary>
    public class SupplierRepository : IGenericRepository<Supplier>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo Repository với chuỗi kết nối
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối (Connection String) để kết nối đến SQL Server.</param>
        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới một bản ghi nhà cung cấp vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="data">Dữ liệu của nhà cung cấp cần thêm</param>
        /// <returns>Mã của nhà cung cấp vừa được thêm (Sinh tự động IDENTITY)</returns>
        public async Task<int> AddAsync(Supplier data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Suppliers(SupplierName, ContactName, Province, Address, Phone, Email)
                               VALUES(@SupplierName, @ContactName, @Province, @Address, @Phone, @Email);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.ExecuteScalarAsync<int>(sql, data);
            }
        }

        /// <summary>
        /// Xóa một nhà cung cấp khỏi cơ sở dữ liệu dựa vào mã (ID).
        /// </summary>
        /// <param name="id">Mã của nhà cung cấp cần xóa</param>
        /// <returns>Trả về True nếu xóa thành công, nếu không thì False.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Suppliers WHERE SupplierID = @id";
                int rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Lấy chi tiết thông tin của một nhà cung cấp theo mã ID.
        /// </summary>
        /// <param name="id">Mã của nhà cung cấp cần lấy</param>
        /// <returns>Đối tượng Supplier hoặc null nếu không tồn tại.</returns>
        public async Task<Supplier?> GetAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Suppliers WHERE SupplierID = @id";
                return await connection.QueryFirstOrDefaultAsync<Supplier>(sql, new { id });
            }
        }

        /// <summary>
        /// Kiểm tra xem nhà cung cấp có đang được sử dụng (có sản phẩm nào liên quan) không.
        /// </summary>
        /// <param name="id">Mã của nhà cung cấp cần kiểm tra</param>
        /// <returns>Trả về True nếu có dữ liệu liên quan, ngược lại là False.</returns>
        public async Task<bool> IsUsedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT CASE WHEN EXISTS(SELECT 1 FROM Products WHERE SupplierID = @id) THEN 1 ELSE 0 END";
                return await connection.ExecuteScalarAsync<bool>(sql, new { id });
            }
        }

        /// <summary>
        /// Tìm kiếm, lấy dữ liệu nhà cung cấp dưới dạng phân trang.
        /// </summary>
        /// <param name="input">Điều kiện tìm kiếm và phân trang (Page, PageSize, SearchValue)</param>
        /// <returns>Trả về đối tượng phân trang chứa danh sách Supplier</returns>
        public async Task<PagedResult<Supplier>> ListAsync(PaginationSearchInput input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Suppliers 
                               WHERE (@searchValue = N'') OR (SupplierName LIKE @searchValue) OR (ContactName LIKE @searchValue)
                               ORDER BY SupplierID
                               OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                               
                               SELECT COUNT(*) FROM Suppliers 
                               WHERE (@searchValue = N'') OR (SupplierName LIKE @searchValue) OR (ContactName LIKE @searchValue);";

                var parameters = new
                {
                    searchValue = string.IsNullOrEmpty(input.SearchValue) ? "" : $"%{input.SearchValue}%",
                    offset = (input.Page - 1) * input.PageSize,
                    pageSize = input.PageSize
                };

                using (var multi = await connection.QueryMultipleAsync(sql, parameters))
                {
                    var data = (await multi.ReadAsync<Supplier>()).ToList();
                    var rowCount = await multi.ReadFirstAsync<int>();

                    // Cập nhật lại thuộc tính RowCount hoặc TotalCount tùy thuộc vào cách bạn định nghĩa PagedResult
                    return new PagedResult<Supplier> 
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
        /// Cập nhật thông tin của một nhà cung cấp đã có sẵn trong CSDL.
        /// </summary>
        /// <param name="data">Dữ liệu nhà cung cấp đã chỉnh sửa</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là False.</returns>
        public async Task<bool> UpdateAsync(Supplier data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Suppliers 
                               SET SupplierName = @SupplierName, ContactName = @ContactName, 
                                   Province = @Province, Address = @Address, Phone = @Phone, Email = @Email
                               WHERE SupplierID = @SupplierID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }
    }
}