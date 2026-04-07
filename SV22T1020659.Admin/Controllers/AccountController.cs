using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020659.Admin;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.HR;
using SV22T1020659.Models.Security;

namespace SV22T1020659.Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            User.GetUserData();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu đi");
                return View();
            }
            string hashedPassword = CryptHelper.HashMD5(password);

            // Lấy thông tin tài khoản thực từ cơ sở dữ liệu
            var userAccount = await UserAccountService.AuthenticateAsync(username, hashedPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập không thành công (Sai tên đăng nhập hoặc mật khẩu)");
                return View();
            }

            // Thông tin đăng nhập hợp lệ:
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames?.Split(',').ToList() ?? new List<string>()
            };

            // Tạo giấy chứng nhận
            var principal = userData.CreatePrincipal();

            // Trao giấy chứng nhận cho phía client
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword,
                                            string newPassword,
                                            string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp");
                return View();
            }

            // Lấy email và thông tin của người dùng đang đăng nhập thông qua GetUserData()
            var userData = User.GetUserData();
            if (userData == null || string.IsNullOrEmpty(userData.Email))
                return RedirectToAction("Login");

            string email = userData.Email;

            // Kiểm tra mật khẩu cũ (phải băm trước khi so sánh)
            var oldHashed = CryptHelper.HashMD5(oldPassword);
            var userAccount = await UserAccountService.AuthenticateAsync(email, oldHashed);
            if (userAccount == null)
            {
                ModelState.AddModelError("", "Mật khẩu cũ không chính xác");
                return View();
            }

            // Thực hiện đổi mật khẩu mới (phải băm trước khi lưu)
            var newHashed = CryptHelper.HashMD5(newPassword);
            bool success = await UserAccountService.ChangePasswordAsync(email, newHashed);
            if (success)
            {
                TempData["Message"] = "Thay đổi mật khẩu thành công!";
                return View(); // Ở lại trang hiện tại và hiển thị thông báo từ TempData
            }

            ModelState.AddModelError("", "Đổi mật khẩu không thành công. Vui lòng thử lại.");
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Đăng ký tài khoản (Admin/Employee)

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(Employee data, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(data.FullName) || string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.Password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ các thông tin bắt buộc");
            }
            if (data.Password != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Xác nhận mật khẩu không khớp");
            }

            if (!ModelState.IsValid)
                return View(data);

            // Kiểm tra email trùng
            bool emailUsed = await HRDataService.ValidateEmployeeEmailAsync(data.Email);
            if (emailUsed)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi một tài khoản khác");
                return View(data);
            }

            // Thiết lập giá trị mặc định cho nhân viên mới
            data.IsWorking = true;
            data.RoleNames = "employee"; // Mặc định là nhân viên
            data.Password = CryptHelper.HashMD5(data.Password!);

            int newId = await HRDataService.AddEmployeeAsync(data);
            if (newId > 0)
            {
                TempData["Message"] = "Đăng ký tài khoản thành công. Bạn có thể đăng nhập ngay.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Đăng ký không thành công. Vui lòng thử lại sau.");
            return View(data);
        }

        #endregion


    }
}