using Microsoft.AspNetCore.Mvc;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Partner;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SV22T1020659.Admin.Controllers
{
    public class CustomerController : Controller
    {/// <summary>
     /// Lưu điều kiện tìm kiếm khách hàng trong session
     /// </summary>
        private const string Customer_search = "CustomerSearchInput";
        /// <summary>
        /// Nhập đầu vào tìm kiếm,Hiển thị kết quả tìm kiếm
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Customer_search);
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
            var result = await PartnerDataService.ListCustomersAsync(input);
            ApplicationContext.SetSessionData(Customer_search, input);
            return View(result);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            var model = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View("Edit", model);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật khách hàng";
            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
          if (Request.Method == "POST")
            {
                await PartnerDataService.DeleteCustomerAsync(id);
                return RedirectToAction("Index");
            }
          //GET 
            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.CanDelete = !await PartnerDataService.IsUsedCustomerAsync(id);
            return View(model);
        }

        // POST: Customer/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await PartnerDataService.DeleteCustomerAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Customer data)
        {
            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật khách hàng";
            // Kiểm tra dữ liệu đầu vào có hợp lệ hay không

            //Sử dụng ModelStae để lư trữ các tình huống (thông báo) lỗi và gửi thông báo lỗi  cho view 
            //Giả thiết: chỉ cần nhập tên, emil và tỉnh thành 
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");

            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email khách hàng không được để trống");
            else if (!await PartnerDataService.ValidatelCustomerEmailAsync(data.Email, data.CustomerID))
                ModelState.AddModelError(nameof(data.Email), "Email khách hàng đã tồn tại");

            if (string.IsNullOrEmpty(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành");

            if (!ModelState.IsValid)
                return View("Edit", data);

            //(Tùy chọn) Hiệu chỉnh dữ liệu theo quy tắc của phần mềm
            if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = data.CustomerName;
            if (string.IsNullOrEmpty(data.Phone)) data.Phone = "";
            if (string.IsNullOrEmpty(data.Address)) data.Address = "";

            //Lưu vào CSDL
            if (data.CustomerID == 0)
            {
                await PartnerDataService.AddCustomerAsync(data);

            }
            else
            {
                await PartnerDataService.UpdateCustomerAsync(data);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Changepassword(int id)
        {
            return View();
        }
    }
}
