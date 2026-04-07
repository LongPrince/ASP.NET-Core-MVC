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

            ViewBag.ProductID = id;
            ViewBag.Photos = await CatalogDataService.ListPhotosAsync(id);
            ViewBag.Attributes = await CatalogDataService.ListAttributesAsync(id);

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

        public async Task<IActionResult> Attribute(int id, string method, long attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    var modelAdd = new ProductAttribute()
                    {
                        ProductID = id,
                        AttributeID = 0
                    };
                    return View("EditAttribute", modelAdd);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    var modelEdit = await CatalogDataService.GetAttributeAsync(attributeId);
                    if (modelEdit == null)
                        return RedirectToAction("Edit", new { id = id });
                    return View("EditAttribute", modelEdit);
                case "delete":
                    await CatalogDataService.DeleteAttributeAsync(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Edit", new { id = id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải lớn hơn 0");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính cho mặt hàng" : "Thay đổi thuộc tính của mặt hàng";
                return View("EditAttribute", data);
            }

            if (data.AttributeID == 0)
                await CatalogDataService.AddAttributeAsync(data);
            else
                await CatalogDataService.UpdateAttributeAsync(data);

            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        // --- Ảnh (Photos) ---
        public async Task<IActionResult> ListPhotos(int id)
        {
            var model = await CatalogDataService.ListPhotosAsync(id);
            return View(model);
        }

        public async Task<IActionResult> Photo(int id, string method, long photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    var modelAdd = new ProductPhoto()
                    {
                        ProductID = id,
                        PhotoID = 0
                    };
                    return View("EditPhoto", modelAdd);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    var modelEdit = await CatalogDataService.GetPhotoAsync(photoId);
                    if (modelEdit == null)
                        return RedirectToAction("Edit", new { id = id });
                    return View("EditPhoto", modelEdit);
                case "delete":
                    await CatalogDataService.DeletePhotoAsync(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Edit", new { id = id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            if (data.PhotoID == 0 && uploadPhoto == null)
                ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn file ảnh");
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải lớn hơn 0");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Thay đổi ảnh của mặt hàng";
                return View("EditPhoto", data);
            }

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

            if (data.PhotoID == 0)
                await CatalogDataService.AddPhotoAsync(data);
            else
                await CatalogDataService.UpdatePhotoAsync(data);

            return RedirectToAction("Edit", new { id = data.ProductID });
        }
    }
}
