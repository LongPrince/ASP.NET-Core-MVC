using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Catalog;
using SV22T1020659.Models.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SV22T1020659.Admin.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private const string Product_search = "ProductSearchInput";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(Product_search);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = 10,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }
            return View(input);
        }

        public async Task<IActionResult> Search(ProductSearchInput input)
        {
            var result = await CatalogDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData(Product_search, input);
            return View(result);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var model = new Product()
            {
                ProductID = 0
            };
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật mặt hàng";
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await CatalogDataService.DeleteProductAsync(id);
                return RedirectToAction("Index");
            }
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.CanDelete = !await CatalogDataService.IsUsedProductAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (data.CategoryID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");

            if (!ModelState.IsValid)
                return View("Edit", data);

            // Xử lý upload ảnh nếu có
            if (uploadPhoto != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
                var folder = Path.Combine(ApplicationContext.WWWRootPath, "images/products");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }
                data.Photo = fileName;
            }

            if (data.ProductID == 0)
                await CatalogDataService.AddProductAsync(data);
            else
                await CatalogDataService.UpdateProductAsync(data);

            return RedirectToAction("Index");
        }

        // --- Thuộc tính (Attributes) ---
        public async Task<IActionResult> ListAttributes(int id)
        {
            var model = await CatalogDataService.ListAttributesAsync(id);
            return View(model);
        }

        // (Thêm các action cho Attribute và Photo tương tự...)
    }
}
