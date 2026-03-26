using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.HR;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SV22T1020659.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var model = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true
            };
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var model = await HRDataService.GetEmployeeAsync(id);
            if (model == null)
                return RedirectToAction("Index");

            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Employee data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.EmployeeID == 0
                    ? "Bổ sung nhân viên"
                    : "Cập nhật thông tin nhân viên";

                // Validate
                if (string.IsNullOrWhiteSpace(data.FullName))
                    ModelState.AddModelError(nameof(data.FullName), "Vui lòng nhập họ tên nhân viên");

                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email nhân viên");
                else if (!await HRDataService.ValidateEmployeeEmailAsync(data.Email, data.EmployeeID))
                    ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng");

                if (!ModelState.IsValid)
                    return View("Edit", data);

                // Upload ảnh
                if (uploadPhoto != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
                    var folder = Path.Combine(ApplicationContext.WWWRootPath, "images/employees");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(stream);
                    }

                    data.Photo = fileName;
                }

                // Default dữ liệu
                data.Address ??= "";
                data.Phone ??= "";
                data.Photo ??= "nophoto.png";

                // Save DB
                if (data.EmployeeID == 0)
                    await HRDataService.AddEmployeeAsync(data);
                else
                    await HRDataService.UpdateEmployeeAsync(data);

                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Hệ thống lỗi, vui lòng thử lại");
                return View("Edit", data);
            }
        }

        public IActionResult Delete(int id)
        {
            return View();
        }

        public IActionResult ChangePassword(int id)
        {
            return View();
        }

        public IActionResult ChangeRole(int id)
        {
            return View();
        }
    }
}