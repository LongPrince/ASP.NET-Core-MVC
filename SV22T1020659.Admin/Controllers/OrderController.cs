using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020659.Admin;
using SV22T1020659.Admin.AppCodes;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Catalog;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Sales;

namespace SV22T1020659.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Sales},{WebUserRoles.Administrator}")]


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
            var userData = User.GetUserData();
            await SalesDataService.AcceptOrderAsync(id, int.Parse(userData.UserId));
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Action thực hiện từ chối đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Reject(int id)
        {
            var userData = User.GetUserData();
            await SalesDataService.RejectOrderAsync(id, int.Parse(userData.UserId));
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
            if (shipperID <= 0)
            {
                TempData["Message"] = "Vui lòng chọn người giao hàng.";
                return RedirectToAction("Detail", new { id });
            }
            await SalesDataService.ShipOrderAsync(id, shipperID);
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Xóa đơn hàng (chỉ cho phép xóa khi đơn hàng đã bị Từ chối hoặc bị Hủy)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");

            if (order.Status != OrderStatusEnum.Rejected && order.Status != OrderStatusEnum.Cancelled)
            {
                TempData["Message"] = "Không thể xóa đơn hàng này do trạng thái hiện tại không cho phép.";
                return RedirectToAction("Detail", new { id });
            }

            await SalesDataService.DeleteOrderAsync(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa một mặt hàng cụ thể ra khỏi chi tiết đơn hàng (chỉ cho phép khi status của đơn hàng là New)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteDetail(int id, int productID)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");

            if (order.Status != OrderStatusEnum.New)
            {
                TempData["Message"] = "Chỉ có thể xóa mặt hàng khi đơn hàng đang ở trạng thái mới.";
                return RedirectToAction("Detail", new { id });
            }

            await SalesDataService.DeleteDetailAsync(id, productID);
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
        public IActionResult AddToCart(OrderDetailViewInfo item)
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
        /// Thêm một mặt hàng vào giỏ hàng (API trả về JSON)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddCartItem(OrderDetailViewInfo item)
        {
            if (item.SalePrice <= 0 || item.Quantity <= 0)
                return Json(new ApiResult(0, "Giá bán và số lượng không hợp lệ"));

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
            return Json(new ApiResult(1, "Thêm vào giỏ hàng thành công"));
        }

        /// <summary>
        /// Hiển thị giao diện xác nhận xóa một mặt hàng khỏi giỏ hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteCartItem(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(m => m.ProductID == id);
            return View(item);
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
        /// Xóa một mặt hàng khỏi giỏ hàng (API trả về JSON)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeleteCartItem")]
        public IActionResult DeleteCartItem_Post(int product)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(m => m.ProductID == product);
            if (item != null)
            {
                cart.Remove(item);
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return Json(new ApiResult(1, "Xóa mặt hàng thành công"));
        }

        /// <summary>
        /// Hiển thị giao diện để cập nhật thông tin mặt hàng trong giỏ hàng hoặc chi tiết đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng (nếu là 0 thì xử lý trên giỏ hàng)</param>
        /// <param name="productID">Mã mặt hàng</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditCartItem(int id = 0, int productID = 0)
        {
            if (id == 0) // Xử lý trên giỏ hàng
            {
                var cart = GetCart();
                var item = cart.FirstOrDefault(m => m.ProductID == productID);
                return View("EditCartItem", item);
            }
            else // Xử lý trên chi tiết đơn hàng đã lưu trong DB
            {
                var order = await SalesDataService.GetOrderAsync(id);
                if (order == null) return RedirectToAction("Index");

                var item = await SalesDataService.GetDetailAsync(id, productID);
                if (item == null) return RedirectToAction("Detail", new { id });

                // Chuyển đổi từ OrderDetail sang OrderDetailViewInfo (nếu cần cho view)
                var product = await CatalogDataService.GetProductAsync(productID);
                var viewInfo = new OrderDetailViewInfo()
                {
                    OrderID = id,
                    ProductID = productID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                    ProductName = product.ProductName,
                    Unit = product.Unit,
                    Photo = product.Photo
                };
                return View("EditCartItem", viewInfo);
            }
        }

        /// <summary>
        /// Cập nhật thông tin mặt hàng trong giỏ hàng (API trả về JSON)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(OrderDetailViewInfo item)
        {
            if (item.Quantity <= 0)
                return Json(new ApiResult(0, "Số lượng không hợp lệ"));

            if (item.OrderID == 0) // Cập nhật trong giỏ hàng (session)
            {
                var cart = GetCart();
                var existingItem = cart.FirstOrDefault(m => m.ProductID == item.ProductID);
                if (existingItem != null)
                {
                    existingItem.Quantity = item.Quantity;
                    existingItem.SalePrice = item.SalePrice;
                    ApplicationContext.SetSessionData(SHOPPING_CART, cart);
                    return Json(new ApiResult(1, "Cập nhật giỏ hàng thành công"));
                }
                return Json(new ApiResult(0, "Mặt hàng không tồn tại trong giỏ hàng"));
            }
            else // Cập nhật trong chi tiết đơn hàng (DB)
            {
                bool success = await SalesDataService.UpdateDetailAsync(new OrderDetail()
                {
                    OrderID = item.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
                if (success)
                    return Json(new ApiResult(1, "Cập nhật chi tiết đơn hàng thành công"));
                else
                    return Json(new ApiResult(0, "Không cập nhật được chi tiết đơn hàng"));
            }
        }

        /// <summary>
        /// Làm trống toàn bộ giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearCart()
        {
            ApplicationContext.SetSessionData(SHOPPING_CART, new List<OrderDetailViewInfo>());
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
                EmployeeID = int.Parse(User.GetUserData().UserId)
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
                ApplicationContext.SetSessionData(SHOPPING_CART, new List<OrderDetailViewInfo>());
                return Json(orderID);
            }

            return Json("Không lập được đơn hàng");
        }

        /// <summary>
        /// Hàm nội bộ đọc dữ liệu giỏ hàng từ session
        /// </summary>
        /// <returns></returns>
        private List<OrderDetailViewInfo> GetCart()
        {
            var cart = ApplicationContext.GetSessionData<List<OrderDetailViewInfo>>(SHOPPING_CART);
            if (cart == null)
            {
                cart = new List<OrderDetailViewInfo>();
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return cart;
        }
    }
}
