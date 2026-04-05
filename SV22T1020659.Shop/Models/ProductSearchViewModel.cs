using SV22T1020659.Models.Catalog;
using SV22T1020659.Models.Common;
using System.Collections.Generic;

namespace SV22T1020659.Shop.Models
{
    /// <summary>
    /// ViewModel cho trang tìm kiếm sản phẩm: bao gồm kết quả tìm kiếm, các mặt hàng hiện tại và bộ lọc danh mục.
    /// </summary>
    public class ProductSearchViewModel : ProductSearchInput
    {
        public PagedResult<Product> Result { get; set; } = new PagedResult<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
