using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020659.Models.Security;
using System.Threading.Tasks;

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
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Error","Vui lòng nhập tên đăng nhập và mật khẩu");
                return View();
            }
            string hashedPassword = CryptHelper.HashMD5(password);
            //TODO: Lấy thông tin tài khoản dựa vào tên đăng nhập và mật khẩu 
            //var userAccount = await SecurityDataService.AuthenticateEmployeeAsync(username, hashedPassword); // Giả sử có một phương thức để xác thực tài khoản nhân viên
            //Giả lập tạm (bỏ và thay bởi đoạn lệnh phía trên TODO)
            var userAccount = new UserAccount()
            {
                UserId = "1",
                UserName = username,
                DisplayName = "Nguyễn Thị Thảo Mai",
                Email = username,
                Photo = "nophoto.png",
                RoleNames = $"{WebUserRoles.Administrator},{WebUserRoles.DataManager}" //admin và datamanager
            };
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View();
            }

            //Thông tin đăng nhập hợp lệ, tiến hành lưu thông tin vào session hoặc cookie
            //Chuẩn bị  thông tin mà sẽ ghi lên chứng nhận 
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()

            };
            //Tạo ra giấy chứng nhận (principal/claims principal) để lưu thông tin đăng nhập
            var principal = userData.CreatePrincipal();
            //Trao giấy chứng nhận cho client  
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
            return RedirectToAction("Index", "Home"); 

        }

        public async Task<IActionResult> logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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


    }
}
