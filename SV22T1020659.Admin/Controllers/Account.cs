using Microsoft.AspNetCore.Mvc;

namespace SV22T1020659.Admin.Controllers
{
    public class Account : Controller
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            return RedirectToAction("Index", "Home");
        }
        public IActionResult logout() 
        { 
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
