using Microsoft.AspNetCore.Mvc;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Catalog;
using SV22T1020659.BusinessLayers;
using System.Threading.Tasks;

namespace SV22T1020659.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private const string Category_search = "CategorySearchInput";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Category_search);
            if (input == null)
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 10,
                    SearchValue = ""
                };
            return View(input);
        }

        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await CatalogDataService.ListCategoriesAsync(input);
            ApplicationContext.SetSessionData(Category_search, input);
            return View(result);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            var model = new Category()
            {
                CategoryID = 0
            };
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật loại hàng";
            var model = await CatalogDataService.GetCategoryAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await CatalogDataService.DeleteCategoryAsync(id);
                return RedirectToAction("Index");
            }
            var model = await CatalogDataService.GetCategoryAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.CanDelete = !await CatalogDataService.IsUsedCategoryAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Category data)
        {
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật loại hàng";

            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được để trống");

            if (!ModelState.IsValid)
                return View("Edit", data);

            if (data.CategoryID == 0)
                await CatalogDataService.AddCategoryAsync(data);
            else
                await CatalogDataService.UpdateCategoryAsync(data);

            return RedirectToAction("Index");
        }
    }
}
