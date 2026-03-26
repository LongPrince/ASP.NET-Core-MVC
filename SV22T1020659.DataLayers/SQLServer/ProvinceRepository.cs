using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using Microsoft.Data.SqlClient;
using SV22T1020659.Models.DataDictionary;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt chức năng lấy danh sách Tỉnh/Thành phố
    /// Kế thừa và triển khai các phương thức từ IDataDictionaryRepository cho kiểu string
    /// </summary>
    public class ProvinceRepository : IDataDictionaryRepository<Province>
    {
        private readonly string _connectionString;

        public ProvinceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Province>> ListAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Giả định bạn có bảng Provinces (ProvinceName) hoặc lấy DISTINCT từ Customers/Suppliers
                // Nếu bạn có bảng cụ thể, hãy thay đổi SQL dưới đây nhé. 
                // Ở đây tôi dùng cách phổ biến khi không có bảng riêng là lấy danh sách các tỉnh thành đã nhập.
                string sql = "SELECT ProvinceName FROM Provinces ORDER BY ProvinceName";
                var result = await connection.QueryAsync<Province>(sql);
                return result.ToList();
            }
        }
    }
}
