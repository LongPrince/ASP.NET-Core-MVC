using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Partner;
using SV22T1020659.Shop.Models;
using System.Security.Claims;

namespace SV22T1020659.Shop.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userAccount = await UserAccountService.AuthenticateAsync(model.Email, model.Password, isEmployee: false);
            if (userAccount == null)
            {
                ModelState.AddModelError(string.Empty, "Đăng nhập thất bại. Vui lòng kiểm tra lại Email và Mật khẩu.");
                return View(model);
            }

            // Authentication success
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userAccount.DisplayName),
                new Claim(ClaimTypes.Email, userAccount.Email),
                new Claim(ClaimTypes.NameIdentifier, userAccount.UserId),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate duplicate email
            bool inUseEmail = await PartnerDataService.ValidatelCustomerEmailAsync(model.Email);
            if (inUseEmail)
            {
                ModelState.AddModelError("Email", "Địa chỉ email này đã được sử dụng.");
                return View(model);
            }

            var provinces = await DictionaryDataService.ListProvincesAsync();
            var defaultProvince = provinces.FirstOrDefault()?.ProvinceName ?? "";

            // Create new Customer
            var customer = new Customer
            {
                CustomerName = model.CustomerName,
                Email = model.Email,
                Phone = model.Phone,
                Password = model.Password,
                ContactName = model.CustomerName, // default contact name
                Province = defaultProvince,
                Address = "",
                IsLocked = false
            };

            int newId = await PartnerDataService.AddCustomerAsync(customer);

            if (newId > 0)
            {
                // Auto login after register
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, customer.CustomerName),
                    new Claim(ClaimTypes.Email, customer.Email),
                    new Claim(ClaimTypes.NameIdentifier, newId.ToString()),
                    new Claim(ClaimTypes.Role, "Customer")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = true });

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình đăng ký. Vui lòng thử lại.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var customer = await PartnerDataService.GetCustomerAsync(userId);
                if (customer != null)
                {
                    var model = new ProfileViewModel
                    {
                        CustomerID = customer.CustomerID,
                        CustomerName = customer.CustomerName,
                        Phone = customer.Phone ?? "",
                        Address = customer.Address ?? "",
                        Province = customer.Province ?? "",
                        ContactName = customer.ContactName
                    };
                    return View(model);
                }
            }
            return RedirectToAction("Login");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId) && userId == model.CustomerID)
            {
                var customer = await PartnerDataService.GetCustomerAsync(userId);
                if (customer != null)
                {
                    customer.CustomerName = model.CustomerName;
                    customer.Phone = model.Phone;
                    customer.Address = model.Address;
                    customer.Province = model.Province;
                    customer.ContactName = model.ContactName;

                    bool success = await PartnerDataService.UpdateCustomerAsync(customer);
                    if (success)
                    {
                        ViewBag.SuccessMessage = "Cập nhật thông tin thành công!";
                        return View(model);
                    }
                }
            }
            
            ModelState.AddModelError(string.Empty, "Cập nhật không thành công.");
            return View(model);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(email))
            {
                // Authenticate to check old password
                var userAccount = await UserAccountService.AuthenticateAsync(email, model.OldPassword, isEmployee: false);
                if (userAccount == null)
                {
                    ModelState.AddModelError("OldPassword", "Mật khẩu hiện tại không đúng.");
                    return View(model);
                }

                // Change password
                bool success = await UserAccountService.ChangePasswordAsync(email, model.NewPassword, isEmployee: false);
                if (success)
                {
                    ViewBag.SuccessMessage = "Đổi mật khẩu thành công!";
                    ModelState.Clear();
                    return View();
                }
            }

            ModelState.AddModelError(string.Empty, "Đổi mật khẩu không thành công.");
            return View(model);
        }
    }
}
