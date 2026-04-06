using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Sales;
using SV22T1020659.Shop.AppCodes;
using System.Security.Claims;

namespace SV22T1020659.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 10;

        /// <summary>
        /// Màn hình xác nhận thông tin giao hàng trước khi đặt
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetCart();
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // Lấy thông tin khách hàng hiện tại
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int customerId))
            {
                var customer = await PartnerDataService.GetCustomerAsync(customerId);
                ViewBag.Customer = customer; // Truyền ra view để set địa chỉ mặc định
            }

            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();

            return View(cart);
        }

        /// <summary>
        /// Xử lý tạo đơn hàng từ giỏ
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Checkout(string deliveryProvince, string deliveryAddress)
        {
            var cart = HttpContext.Session.GetCart();
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Tạo Order
            Order order = new Order()
            {
                CustomerID = customerId,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress
            };

            int orderId = await SalesDataService.AddOrderAsync(order);

            // Tạo OrderDetails
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail()
                {
                    OrderID = orderId,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                };
                await SalesDataService.AddDetailAsync(detail);
            }

            // Clear giỏ hàng toàn bộ
            HttpContext.Session.ClearCart();

            return RedirectToAction("Payment", new { id = orderId });
        }

        /// <summary>
        /// Hiển thị màn hình Thanh toán VietQR
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Payment(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.CustomerID.ToString() != userIdStr) return Forbid();

            // Tính tổng tiền
            var details = await SalesDataService.ListDetailsAsync(id);
            decimal totalAmount = details.Sum(x => x.TotalPrice);

            ViewBag.TotalAmount = totalAmount;
            return View(order);
        }

        /// <summary>
        /// Danh sách lịch sử mua hàng của Customer hiện tại
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            
            OrderSearchInput input = new OrderSearchInput()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = "",
                Status = 0, // Tất cả trạng thái
                CustomerID = customerId
            };

            var data = await SalesDataService.ListOrdersAsync(input);
            return View(data);
        }

        /// <summary>
        /// Chi tiết một đơn hàng của Customer
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.CustomerID.ToString() != userIdStr) return Forbid();

            var details = await SalesDataService.ListDetailsAsync(id);
            ViewBag.Details = details;

            return View(order);
        }
    }
}
