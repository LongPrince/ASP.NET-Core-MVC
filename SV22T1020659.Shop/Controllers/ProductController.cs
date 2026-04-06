using Microsoft.AspNetCore.Mvc;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Catalog;
using SV22T1020659.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SV22T1020659.Shop.Controllers
{
    /// <summary>
    /// Controller xử lý các chức năng liên quan đến Sản phẩm cho khách hàng.
    /// </summary>
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 12; // Số sản phẩm trên mỗi trang

        /// <summary>
        /// Trang danh sách sản phẩm (Tìm kiếm & Lọc)
        /// </summary>
        public async Task<IActionResult> Index(ProductSearchInput input)
        {
            // Thiết lập giá trị mặc định cho phân trang
            if (input.Page <= 0) input.Page = 1;
            if (input.PageSize <= 0) input.PageSize = PAGE_SIZE;

            // Truy xuất danh sách sản phẩm có phân trang
            var productResult = await CatalogDataService.ListProductsAsync(input);

            // Lấy danh sách tất cả các loại hàng cho sidebar
            var categoryResult = await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput
            {
                Page = 1,
                PageSize = 999, // Lấy toàn bộ danh mục cho sidebar
                SearchValue = ""
            });

            // Lưu vào ViewBag thay vì ViewModel tùy chỉnh
            ViewBag.SearchValue = input.SearchValue ?? "";
            ViewBag.CategoryID = input.CategoryID;
            ViewBag.SupplierID = input.SupplierID;
            ViewBag.MinPrice = input.MinPrice;
            ViewBag.MaxPrice = input.MaxPrice;
            ViewBag.Categories = categoryResult.DataItems;

            return View(productResult);
        }

        /// <summary>
        /// Xem chi tiết sản phẩm
        /// </summary>
        public async Task<IActionResult> Detail(int id)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null)
                return RedirectToAction("Index");

            // Lấy thêm ảnh và thuộc tính
            ViewBag.Photos = await CatalogDataService.ListPhotosAsync(id);
            ViewBag.Attributes = await CatalogDataService.ListAttributesAsync(id);

            return View(product);
        }
    }
}
