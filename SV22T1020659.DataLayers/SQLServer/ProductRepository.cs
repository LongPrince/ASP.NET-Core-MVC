using Dapper;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Catalog;
using SV22T1020659.Models.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt chức năng xử lý dữ liệu cho mặt hàng, ảnh và thuộc tính của mặt hàng trên SQL Server
    /// Kế thừa và triển khai các phương thức từ IProductRepository
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo Repository với chuỗi kết nối
        /// </summary>
        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Mặt hàng (Product)

        public async Task<int> AddAsync(Product data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                               VALUES(@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.ExecuteScalarAsync<int>(sql, data);
            }
        }

        public async Task<bool> DeleteAsync(int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Xóa các thuộc tính của mặt hàng
                        await connection.ExecuteAsync("DELETE FROM ProductAttributes WHERE ProductID = @productID", new { productID }, transaction);
                        // Xóa các ảnh của mặt hàng
                        await connection.ExecuteAsync("DELETE FROM ProductPhotos WHERE ProductID = @productID", new { productID }, transaction);
                        // Xóa mặt hàng
                        string sql = "DELETE FROM Products WHERE ProductID = @productID";
                        int rowsAffected = await connection.ExecuteAsync(sql, new { productID }, transaction);

                        transaction.Commit();
                        return rowsAffected > 0;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<Product?> GetAsync(int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Products WHERE ProductID = @productID";
                return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { productID });
            }
        }

        public async Task<bool> IsUsedAsync(int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT CASE WHEN EXISTS(SELECT 1 FROM OrderDetails WHERE ProductID = @productID) THEN 1 ELSE 0 END";
                return await connection.ExecuteScalarAsync<bool>(sql, new { productID });
            }
        }

        public async Task<PagedResult<Product>> ListAsync(ProductSearchInput input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Products 
                               WHERE (@searchValue = N'' OR ProductName LIKE @searchValue)
                                 AND (@categoryID = 0 OR CategoryID = @categoryID)
                                 AND (@supplierID = 0 OR SupplierID = @supplierID)
                                 AND (@minPrice = 0 OR Price >= @minPrice)
                                 AND (@maxPrice = 0 OR Price <= @maxPrice)
                               ORDER BY ProductID
                               OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                               
                               SELECT COUNT(*) FROM Products 
                               WHERE (@searchValue = N'' OR ProductName LIKE @searchValue)
                                 AND (@categoryID = 0 OR CategoryID = @categoryID)
                                 AND (@supplierID = 0 OR SupplierID = @supplierID)
                                 AND (@minPrice = 0 OR Price >= @minPrice)
                                 AND (@maxPrice = 0 OR Price <= @maxPrice);";

                var parameters = new
                {
                    searchValue = string.IsNullOrEmpty(input.SearchValue) ? "" : $"%{input.SearchValue}%",
                    categoryID = input.CategoryID,
                    supplierID = input.SupplierID,
                    minPrice = input.MinPrice,
                    maxPrice = input.MaxPrice,
                    offset = (input.Page - 1) * input.PageSize,
                    pageSize = input.PageSize
                };

                using (var multi = await connection.QueryMultipleAsync(sql, parameters))
                {
                    var data = (await multi.ReadAsync<Product>()).ToList();
                    var rowCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<Product>
                    {
                        DataItems = data,
                        RowCount = rowCount,
                        Page = input.Page,
                        PageSize = input.PageSize
                    };
                }
            }
        }

        public async Task<bool> UpdateAsync(Product data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Products 
                               SET ProductName = @ProductName, 
                                   ProductDescription = @ProductDescription, 
                                   SupplierID = @SupplierID, 
                                   CategoryID = @CategoryID, 
                                   Unit = @Unit, 
                                   Price = @Price, 
                                   Photo = @Photo, 
                                   IsSelling = @IsSelling
                               WHERE ProductID = @ProductID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }

        #endregion

        #region Thuộc tính mặt hàng (ProductAttribute)

        public async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                               VALUES(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                               SELECT CAST(SCOPE_IDENTITY() as bigint);";
                return await connection.ExecuteScalarAsync<long>(sql, data);
            }
        }

        public async Task<bool> DeleteAttributeAsync(long attributeID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM ProductAttributes WHERE AttributeID = @attributeID";
                int rowsAffected = await connection.ExecuteAsync(sql, new { attributeID });
                return rowsAffected > 0;
            }
        }

        public async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM ProductAttributes WHERE AttributeID = @attributeID";
                return await connection.QueryFirstOrDefaultAsync<ProductAttribute>(sql, new { attributeID });
            }
        }

        public async Task<List<ProductAttribute>> ListAttributesAsync(int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM ProductAttributes WHERE ProductID = @productID ORDER BY DisplayOrder";
                return (await connection.QueryAsync<ProductAttribute>(sql, new { productID })).ToList();
            }
        }

        public async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE ProductAttributes 
                               SET ProductID = @ProductID, 
                                   AttributeName = @AttributeName, 
                                   AttributeValue = @AttributeValue, 
                                   DisplayOrder = @DisplayOrder
                               WHERE AttributeID = @AttributeID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }

        #endregion

        #region Ảnh mặt hàng (ProductPhoto)

        public async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                               VALUES(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                               SELECT CAST(SCOPE_IDENTITY() as bigint);";
                return await connection.ExecuteScalarAsync<long>(sql, data);
            }
        }

        public async Task<bool> DeletePhotoAsync(long photoID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM ProductPhotos WHERE PhotoID = @photoID";
                int rowsAffected = await connection.ExecuteAsync(sql, new { photoID });
                return rowsAffected > 0;
            }
        }

        public async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM ProductPhotos WHERE PhotoID = @photoID";
                return await connection.QueryFirstOrDefaultAsync<ProductPhoto>(sql, new { photoID });
            }
        }

        public async Task<List<ProductPhoto>> ListPhotosAsync(int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM ProductPhotos WHERE ProductID = @productID ORDER BY DisplayOrder";
                return (await connection.QueryAsync<ProductPhoto>(sql, new { productID })).ToList();
            }
        }

        public async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE ProductPhotos 
                               SET ProductID = @ProductID, 
                                   Photo = @Photo, 
                                   Description = @Description, 
                                   DisplayOrder = @DisplayOrder, 
                                   IsHidden = @IsHidden
                               WHERE PhotoID = @PhotoID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }

        #endregion
    }
}
