using SV22T1020659.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1020659.Models.Catalog
{
    /// <summary>
    /// Biểu diễn dữ liệu đầu vào tìm kiếm, phân trang đối với mặt hàng
    /// </summary>
    public class ProductSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// Mã loại hàng (0 nếu bỏ qua)
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// Mã nhà cung cấp (0 nếu bỏ qua)
        /// </summary>
        public int SupplierID { get; set; }
        /// <summary>
        /// Giá tối thiểu (0 nếu bỏ qua)
        /// </summary>
        public decimal MinPrice { get; set; }
        /// <summary>
        /// Mức giá tối đa (0 nếu bỏ qua)
        /// </summary>
        public decimal MaxPrice { get; set; }
    }
}
