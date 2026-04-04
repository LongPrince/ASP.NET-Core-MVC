using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SV22T1020659.Models.Security;
using SV22T1020659.Admin;
using SV22T1020659.Models.Security;
using SV22T1020659.BusinessLayers;

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
            var userAccount = await UserAccountService.AuthenticateAsync(username, password);
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
        public IActionResult ChangePassword(string oldPassword,
                                            string newPassword,
                                            string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp");
                return View();
            }

            // TODO: xử lý đổi mật khẩu
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}