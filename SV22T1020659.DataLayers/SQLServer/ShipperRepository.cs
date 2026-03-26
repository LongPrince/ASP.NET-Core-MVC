using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Partner;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho Người giao hàng trên SQL Server
    /// Kế thừa và triển khai các phương thức từ IGenericRepository.
    /// </summary>
    public class ShipperRepository : IGenericRepository<Shipper>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo Repository với chuỗi kết nối
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối (Connection String) để kết nối đến SQL Server.</param>
        public ShipperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới một bản ghi người giao hàng vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="data">Dữ liệu của người giao hàng cần thêm</param>
        /// <returns>Mã của người giao hàng vừa được thêm (Sinh tự động IDENTITY)</returns>
        public async Task<int> AddAsync(Shipper data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Shippers(ShipperName, Phone)
                               VALUES(@ShipperName, @Phone);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.ExecuteScalarAsync<int>(sql, data);
            }
        }

        /// <summary>
        /// Xóa một người giao hàng khỏi cơ sở dữ liệu dựa vào mã (ID).
        /// </summary>
        /// <param name="id">Mã của người giao hàng cần xóa</param>
        /// <returns>Trả về True nếu xóa thành công, nếu không thì False.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Shippers WHERE ShipperID = @id";
                int rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Lấy chi tiết thông tin của một người giao hàng theo mã ID.
        /// </summary>
        /// <param name="id">Mã của người giao hàng cần lấy</param>
        /// <returns>Đối tượng Shipper hoặc null nếu không tồn tại.</returns>
        public async Task<Shipper?> GetAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Shippers WHERE ShipperID = @id";
                return await connection.QueryFirstOrDefaultAsync<Shipper>(sql, new { id });
            }
        }

        /// <summary>
        /// Kiểm tra xem người giao hàng có đang được sử dụng (có đơn hàng nào liên quan) không.
        /// </summary>
        /// <param name="id">Mã của người giao hàng cần kiểm tra</param>
        /// <returns>Trả về True nếu có dữ liệu liên quan, ngược lại là False.</returns>
        public async Task<bool> IsUsedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Giả định bảng Orders có trường ShipperID. Nếu thư viện của bạn thiết kế bảng khác hãy đổi lại "Orders"
                string sql = "SELECT CASE WHEN EXISTS(SELECT 1 FROM Orders WHERE ShipperID = @id) THEN 1 ELSE 0 END";
                return await connection.ExecuteScalarAsync<bool>(sql, new { id });
            }
        }

        /// <summary>
        /// Tìm kiếm, lấy dữ liệu người giao hàng dưới dạng phân trang.
        /// </summary>
        /// <param name="input">Điều kiện tìm kiếm và phân trang (Page, PageSize, SearchValue)</param>
        /// <returns>Trả về đối tượng phân trang chứa danh sách Shipper</returns>
        public async Task<PagedResult<Shipper>> ListAsync(PaginationSearchInput input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Shippers 
                               WHERE (@searchValue = N'') OR (ShipperName LIKE @searchValue) OR (Phone LIKE @searchValue)
                               ORDER BY ShipperID
                               OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                               
                               SELECT COUNT(*) FROM Shippers 
                               WHERE (@searchValue = N'') OR (ShipperName LIKE @searchValue) OR (Phone LIKE @searchValue);";

                var parameters = new
                {
                    searchValue = string.IsNullOrEmpty(input.SearchValue) ? "" : $"%{input.SearchValue}%",
                    offset = (input.Page - 1) * input.PageSize,
                    pageSize = input.PageSize
                };

                using (var multi = await connection.QueryMultipleAsync(sql, parameters))
                {
                    var data = (await multi.ReadAsync<Shipper>()).ToList();
                    var rowCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<Shipper> 
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
        /// Cập nhật thông tin của một người giao hàng đã có sẵn trong CSDL.
        /// </summary>
        /// <param name="data">Dữ liệu người giao hàng đã chỉnh sửa</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là False.</returns>
        public async Task<bool> UpdateAsync(Shipper data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Shippers 
                               SET ShipperName = @ShipperName, Phone = @Phone
                               WHERE ShipperID = @ShipperID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }
    }
}
