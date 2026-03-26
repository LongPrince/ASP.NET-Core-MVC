using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Catalog;
using SV22T1020659.Models.Common;
using System.Data;

namespace SV22T1020659.Datalayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho Loại hàng (Category)
    /// </summary>
    public class CategoryRepository : IGenericRepository<Category>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo Repository với chuỗi kết nối
        /// </summary>
        /// <param name="connectionString"></param>
        public CategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Bổ sung một loại hàng mới
        /// </summary>
        public async Task<int> AddAsync(Category data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Categories (CategoryName, Description)
                               VALUES (@CategoryName, @Description);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.ExecuteScalarAsync<int>(sql, data);
            }
        }

        /// <summary>
        /// Xóa loại hàng theo mã ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Categories WHERE CategoryID = @CategoryID";
                int rowsAffected = await connection.ExecuteAsync(sql, new { CategoryID = id });
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Lấy thông tin một loại hàng theo ID
        /// </summary>
        public async Task<Category?> GetAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Categories WHERE CategoryID = @CategoryID";
                return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { CategoryID = id });
            }
        }

        /// <summary>
        /// Kiểm tra xem loại hàng đã có mặt hàng (Products) nào thuộc về nó chưa
        /// </summary>
        public async Task<bool> IsUsedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT CASE WHEN EXISTS(SELECT 1 FROM Products WHERE CategoryID = @CategoryID) THEN 1 ELSE 0 END";
                return await connection.ExecuteScalarAsync<bool>(sql, new { CategoryID = id });
            }
        }

        /// <summary>
        /// Tìm kiếm và phân trang danh sách loại hàng
        /// </summary>
        public async Task<PagedResult<Category>> ListAsync(PaginationSearchInput input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT * FROM Categories 
                            WHERE (@SearchValue = N'') OR (CategoryName LIKE @SearchValue)
                            ORDER BY CategoryID 
                            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

                            SELECT COUNT(*) FROM Categories 
                            WHERE (@SearchValue = N'') OR (CategoryName LIKE @SearchValue);";

                var searchValue = string.IsNullOrEmpty(input.SearchValue) ? "" : $"%{input.SearchValue}%";

                using (var multi = await connection.QueryMultipleAsync(sql, new
                {
                    SearchValue = searchValue,
                    Page = input.Page,
                    PageSize = input.PageSize
                }))
                {
                    // Chú ý: data phải là List<Category>
                    var data = (await multi.ReadAsync<Category>()).ToList();
                    var rowCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<Category>
                    {
                        Page = input.Page,
                        PageSize = input.PageSize,
                        RowCount = rowCount,
                        DataItems = data
                    };
                }
            }
        }

        /// <summary>
        /// Cập nhật thông tin loại hàng
        /// </summary>
        public async Task<bool> UpdateAsync(Category data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Categories 
                               SET CategoryName = @CategoryName, 
                                   Description = @Description
                               WHERE CategoryID = @CategoryID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }
    }
}