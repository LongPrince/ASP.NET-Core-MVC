using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Sales;

namespace SV22T1020659.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt chức năng xử lý dữ liệu cho đơn hàng và các mặt hàng bán trong đơn hàng trên SQL Server
    /// Kế thừa và triển khai các phương thức từ IOrderRepository
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo Repository với chuỗi kết nối
        /// </summary>
        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Đơn hàng (Order)

        public async Task<int> AddAsync(Order data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Orders(CustomerID, OrderTime, DeliveryProvince, DeliveryAddress, EmployeeID, AcceptTime, ShipperID, ShippedTime, FinishedTime, Status)
                               VALUES(@CustomerID, @OrderTime, @DeliveryProvince, @DeliveryAddress, @EmployeeID, @AcceptTime, @ShipperID, @ShippedTime, @FinishedTime, @Status);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.ExecuteScalarAsync<int>(sql, data);
            }
        }

        public async Task<bool> DeleteAsync(int orderID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Orders WHERE OrderID = @orderID";
                int rowsAffected = await connection.ExecuteAsync(sql, new { orderID });
                return rowsAffected > 0;
            }
        }

        public async Task<OrderViewInfo?> GetAsync(int orderID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT o.*, 
                                      e.FullName as EmployeeName, 
                                      c.CustomerName as CustomerName, 
                                      c.ContactName as CustomerContactName,
                                      c.Email as CustomerEmail,
                                      c.Phone as CustomerPhone,
                                      CONCAT(c.Address, ', ', c.Province) as CustomerAddress,
                                      s.ShipperName as ShipperName, 
                                      s.Phone as ShipperPhone
                               FROM Orders o
                               LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                               LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                               LEFT JOIN Shippers s ON o.ShipperID = s.ShipperID
                               WHERE o.OrderID = @orderID";
                return await connection.QueryFirstOrDefaultAsync<OrderViewInfo>(sql, new { orderID });
            }
        }

        public async Task<PagedResult<OrderViewInfo>> ListAsync(OrderSearchInput input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT o.*, 
                                      e.FullName as EmployeeName, 
                                      c.CustomerName as CustomerName, 
                                      c.ContactName as CustomerContactName,
                                      c.Email as CustomerEmail,
                                      c.Phone as CustomerPhone,
                                      CONCAT(c.Address, ', ', c.Province) as CustomerAddress,
                                      s.ShipperName as ShipperName, 
                                      s.Phone as ShipperPhone
                               FROM Orders o
                               LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                               LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                               LEFT JOIN Shippers s ON o.ShipperID = s.ShipperID
                               WHERE (@status = 0 OR o.Status = @status)
                                 AND (@dateFrom IS NULL OR o.OrderTime >= @dateFrom)
                                 AND (@dateTo IS NULL OR o.OrderTime <= @dateTo)
                                 AND (@customerID = 0 OR o.CustomerID = @customerID)
                                 AND (@searchValue = N'' OR 
                                      c.CustomerName LIKE @searchValue OR 
                                      s.ShipperName LIKE @searchValue)
                                ORDER BY o.OrderID DESC
                                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                                
                                SELECT COUNT(*) FROM Orders o
                                LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                                LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                                LEFT JOIN Shippers s ON o.ShipperID = s.ShipperID
                                WHERE (@status = 0 OR o.Status = @status)
                                  AND (@dateFrom IS NULL OR o.OrderTime >= @dateFrom)
                                  AND (@dateTo IS NULL OR o.OrderTime <= @dateTo)
                                  AND (@customerID = 0 OR o.CustomerID = @customerID)
                                  AND (@searchValue = N'' OR 
                                       c.CustomerName LIKE @searchValue OR 
                                       s.ShipperName LIKE @searchValue);";

                var parameters = new
                {
                    status = (int)input.Status,
                    dateFrom = input.DateFrom,
                    dateTo = input.DateTo,
                    customerID = input.CustomerID,
                    searchValue = string.IsNullOrEmpty(input.SearchValue) ? "" : $"%{input.SearchValue}%",
                    offset = (input.Page - 1) * input.PageSize,
                    pageSize = input.PageSize
                };

                using (var multi = await connection.QueryMultipleAsync(sql, parameters))
                {
                    var data = (await multi.ReadAsync<OrderViewInfo>()).ToList();
                    var rowCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<OrderViewInfo>
                    {
                        DataItems = data,
                        RowCount = rowCount,
                        Page = input.Page,
                        PageSize = input.PageSize
                    };
                }
            }
        }

        public async Task<bool> UpdateAsync(Order data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Orders 
                               SET CustomerID = @CustomerID, 
                                   OrderTime = @OrderTime, 
                                   DeliveryProvince = @DeliveryProvince, 
                                   DeliveryAddress = @DeliveryAddress, 
                                   EmployeeID = @EmployeeID, 
                                   AcceptTime = @AcceptTime, 
                                   ShipperID = @ShipperID, 
                                   ShippedTime = @ShippedTime, 
                                   FinishedTime = @FinishedTime, 
                                   Status = @Status
                               WHERE OrderID = @OrderID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }

        #endregion

        #region Chi tiết đơn hàng (OrderDetail)

        public async Task<bool> AddDetailAsync(OrderDetail data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice)
                               VALUES(@OrderID, @ProductID, @Quantity, @SalePrice)";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteDetailAsync(int orderID, int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM OrderDetails WHERE OrderID = @orderID AND ProductID = @productID";
                int rowsAffected = await connection.ExecuteAsync(sql, new { orderID, productID });
                return rowsAffected > 0;
            }
        }

        public async Task<OrderDetailViewInfo?> GetDetailAsync(int orderID, int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT od.*, p.ProductName, p.Unit, p.Photo
                               FROM OrderDetails od
                               JOIN Products p ON od.ProductID = p.ProductID
                               WHERE od.OrderID = @orderID AND od.ProductID = @productID";
                return await connection.QueryFirstOrDefaultAsync<OrderDetailViewInfo>(sql, new { orderID, productID });
            }
        }

        public async Task<List<OrderDetailViewInfo>> ListDetailsAsync(int orderID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT od.*, p.ProductName, p.Unit, p.Photo
                               FROM OrderDetails od
                               JOIN Products p ON od.ProductID = p.ProductID
                               WHERE od.OrderID = @orderID";
                return (await connection.QueryAsync<OrderDetailViewInfo>(sql, new { orderID })).ToList();
            }
        }

        public async Task<bool> UpdateDetailAsync(OrderDetail data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE OrderDetails 
                               SET Quantity = @Quantity, 
                                   SalePrice = @SalePrice
                               WHERE OrderID = @OrderID AND ProductID = @ProductID";
                int rowsAffected = await connection.ExecuteAsync(sql, data);
                return rowsAffected > 0;
            }
        }

        #endregion
    }
}
