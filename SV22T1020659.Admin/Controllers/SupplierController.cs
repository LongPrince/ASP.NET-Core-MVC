using Microsoft.AspNetCore.Mvc;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Partner;
using System.Reflection;
using System.Threading.Tasks;

namespace SV22T1020659.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private const string Supplier_search = "SupplierSearchInput";

        /// <summary>
        /// Giao diện tìm kiếm nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Supplier_search);
            if (input == null)
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 5,
                    SearchValue = ""
                };
            return View(input);
        }

        /// <summary>
        /// Tìm kiếm và trả về kết quả
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListSuppliersAsync(input);
            ApplicationContext.SetSessionData(Supplier_search, input);
            return View(result);
        }

        // GET: Supplier/Create
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            var model = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit", model);
        }

        // GET: Supplier/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật nhà cung cấp";
            var model = await PartnerDataService.GetSupplierAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        // GET: Supplier/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await PartnerDataService.DeleteSupplierAsync(id);
                return RedirectToAction("Index");
            }
            //GET 
            var model = await PartnerDataService.GetSupplierAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.CanDelete = !await PartnerDataService.IsUsedSupplierAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Supplier data)
        {
            ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật nhà cung cấp";

            if (string.IsNullOrWhiteSpace(data.SupplierName))
                ModelState.AddModelError(nameof(data.SupplierName), "Tên nhà cung cấp không được để trống");

            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");

            if (string.IsNullOrEmpty(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành");

            if (!ModelState.IsValid)
                return View("Edit", data);

            // Hiệu chỉnh dữ liệu theo quy tắc
            if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = data.SupplierName;
            if (string.IsNullOrEmpty(data.Phone)) data.Phone = "";
            if (string.IsNullOrEmpty(data.Address)) data.Address = "";

            // Lưu vào CSDL
            if (data.SupplierID == 0)
            {
                await PartnerDataService.AddSupplierAsync(data);
            }
            else
            {
                await PartnerDataService.UpdateSupplierAsync(data);
            }
            return RedirectToAction("Index");
        }
    }
}
