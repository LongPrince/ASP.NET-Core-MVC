using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Sales;
using SV22T1020659.Shop.AppCodes;

namespace SV22T1020659.Shop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Số lượng không hợp lệ" });

            var cart = HttpContext.Session.GetCart();
            var item = cart.FirstOrDefault(m => m.ProductID == id);

            if (item == null)
            {
                var product = await CatalogDataService.GetProductAsync(id);
                if (product == null)
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });

                item = new CartItem()
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName ?? "",
                    Photo = product.Photo ?? "",
                    Unit = product.Unit ?? "",
                    SalePrice = product.Price,
                    Quantity = quantity
                };
                cart.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            HttpContext.Session.SetCart(cart);
            return Json(new { success = true, message = "Đã thêm vào giỏ hàng", cartCount = cart.Sum(m => m.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateCart(int id, int quantity)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Số lượng không hợp lệ" });

            var cart = HttpContext.Session.GetCart();
            var item = cart.FirstOrDefault(m => m.ProductID == id);

            if (item != null)
            {
                item.Quantity = quantity;
                HttpContext.Session.SetCart(cart);
                return Json(new { success = true, message = "Đã cập nhật giỏ hàng", cartCount = cart.Sum(m => m.Quantity), itemTotal = item.TotalPrice.ToString("N0") });
            }

            return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng" });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetCart();
            var item = cart.FirstOrDefault(m => m.ProductID == id);

            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetCart(cart);
                return Json(new { success = true, message = "Đã xóa sản phẩm khỏi giỏ hàng", cartCount = cart.Sum(m => m.Quantity) });
            }

            return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng" });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.ClearCart();
            return Json(new { success = true, message = "Đã làm trống giỏ hàng" });
        }
    }
}
