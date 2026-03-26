using Microsoft.AspNetCore.Mvc;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Sales;
using SV22T1020659.Models.Catalog;
using SV22T1020659.BusinessLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SV22T1020659.Admin.Controllers
{
    /// <summary>
    /// Controller quản lý đơn hàng
    /// </summary>
    public class OrderController : Controller
    {
        private const string Order_search = "OrderSearchInput";
        private const string SHOPPING_CART = "ShoppingCart";

        /// <summary>
        /// Trang chủ quản lý đơn hàng: Nhập thông tin tìm kiếm, phân trang và hiển thị đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<OrderSearchInput>(Order_search);
            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = 20,
                    SearchValue = "",
                    Status = 0,
                    DateFrom = null,
                    DateTo = null
                };
            }
            return View(input);
        }

        /// <summary>
        /// Thực hiện tìm kiếm và phân trang đơn hàng bằng AJAX
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IActionResult> Search(OrderSearchInput input)
        {
            var result = await SalesDataService.ListOrdersAsync(input);
            ApplicationContext.SetSessionData(Order_search, input);
            return View(result);
        }

        /// <summary>
        /// Hiển thị thông tin chi tiết đơn hàng (các thông tin liên quan, danh sách mặt hàng đã mua)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = await SalesDataService.ListDetailsAsync(id);
            ViewBag.OrderDetails = details;

            return View(order);
        }

        /// <summary>
        /// Action thực hiện duyệt (chấp nhận) đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Accept(int id)
        {
            // Tạm thời gán EmployeeID = 1 cho đến khi có Authentication
            await SalesDataService.AcceptOrderAsync(id, 1);
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Action thực hiện từ chối đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Reject(int id)
        {
            await SalesDataService.RejectOrderAsync(id, 1);
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Action thực hiện hủy bỏ đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Cancel(int id)
        {
            await SalesDataService.CancelOrderAsync(id);
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Action thực hiện hoàn tất (kết thúc) đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Finish(int id)
        {
            await SalesDataService.CompleteOrderAsync(id);
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Hiển thị giao diện để chọn người giao hàng (Shipper)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Shipping(int id)
        {
            ViewBag.OrderID = id;
            return View();
        }

        /// <summary>
        /// Thực hiện lưu trữ thông tin người giao hàng và chuyển trạng thái đơn hàng sang Đang giao
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Shipping(int id, int shipperID)
        {
            await SalesDataService.ShipOrderAsync(id, shipperID);
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Giao diện hỗ trợ nghiệp vụ tạo một đơn hàng mới (Trang bán hàng)
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var cart = GetCart();
            return View(cart);
        }

        /// <summary>
        /// Tìm kiếm mặt hàng theo tên và phân trang để lựa chọn đưa vào giỏ hàng
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IActionResult> SearchProduct(string searchValue = "", int page = 1)
        {
            int pageSize = 5;
            var input = new ProductSearchInput()
            {
                Page = page,
                PageSize = pageSize,
                SearchValue = searchValue,
                CategoryID = 0,
                SupplierID = 0
            };
            var result = await CatalogDataService.ListProductsAsync(input);
            return View(result);
        }

        /// <summary>
        /// Thêm một mặt hàng được chọn vào giỏ hàng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice <= 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existingItem == null)
            {
                cart.Add(item);
            }
            else
            {
                existingItem.Quantity += item.Quantity;
                existingItem.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return RedirectToAction("ShowCart");
        }

        /// <summary>
        /// Xóa một mặt hàng khỏi giỏ hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(m => m.ProductID == id);
            if (existingItem != null)
                cart.Remove(existingItem);

            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return RedirectToAction("ShowCart");
        }

        /// <summary>
        /// Làm trống toàn bộ giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearCart()
        {
            ApplicationContext.SetSessionData(SHOPPING_CART, new List<CartItem>());
            return RedirectToAction("ShowCart");
        }

        /// <summary>
        /// Trả về partial hiển thị giỏ hàng hiện tại (Sử dụng cho AJAX)
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowCart()
        {
            var cart = GetCart();
            return View(cart);
        }

        /// <summary>
        /// Khởi tạo và lưu đơn hàng vào cơ sở dữ liệu dựa trên giỏ hàng và thông tin khách hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="deliveryProvince"></param>
        /// <param name="deliveryAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Init(int customerID, string deliveryProvince, string deliveryAddress)
        {
            var cart = GetCart();
            if (cart.Count == 0)
                return Json("Giỏ hàng đang trống");

            if (customerID <= 0 || string.IsNullOrEmpty(deliveryProvince) || string.IsNullOrEmpty(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi nhận hàng");

            int orderID = await SalesDataService.AddOrderAsync(new Order()
            {
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                EmployeeID = 1 // Gán tạm thời nhân viên login
            });

            if (orderID > 0)
            {
                foreach (var item in cart)
                {
                    await SalesDataService.AddDetailAsync(new OrderDetail()
                    {
                        OrderID = orderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        SalePrice = item.SalePrice
                    });
                }
                ApplicationContext.SetSessionData(SHOPPING_CART, new List<CartItem>());
                return Json(orderID);
            }

            return Json("Không lập được đơn hàng");
        }

        /// <summary>
        /// Hàm nội bộ đọc dữ liệu giỏ hàng từ session
        /// </summary>
        /// <returns></returns>
        private List<CartItem> GetCart()
        {
            var cart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (cart == null)
            {
                cart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return cart;
        }
    }
}
