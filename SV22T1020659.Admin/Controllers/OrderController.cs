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
    public class OrderController : Controller
    {
        private const string Order_search = "OrderSearchInput";
        private const string SHOPPING_CART = "ShoppingCart";

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

        public async Task<IActionResult> Search(OrderSearchInput input)
        {
            var result = await SalesDataService.ListOrdersAsync(input);
            ApplicationContext.SetSessionData(Order_search, input);
            return View(result);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = await SalesDataService.ListDetailsAsync(id);
            ViewBag.OrderDetails = details;

            return View(order);
        }

        public async Task<IActionResult> Accept(int id)
        {
            await SalesDataService.AcceptOrderAsync(id, 1);
            return RedirectToAction("Detail", new { id });
        }

        public async Task<IActionResult> Reject(int id)
        {
            await SalesDataService.RejectOrderAsync(id, 1);
            return RedirectToAction("Detail", new { id });
        }

        public async Task<IActionResult> Cancel(int id)
        {
            await SalesDataService.CancelOrderAsync(id);
            return RedirectToAction("Detail", new { id });
        }

        public async Task<IActionResult> Finish(int id)
        {
            await SalesDataService.CompleteOrderAsync(id);
            return RedirectToAction("Detail", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Shipping(int id)
        {
            ViewBag.OrderID = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Shipping(int id, int shipperID)
        {
            await SalesDataService.ShipOrderAsync(id, shipperID);
            return RedirectToAction("Detail", new { id });
        }

        // --- Lập đơn hàng mới ---

        public IActionResult Create()
        {
            var cart = GetCart();
            return View(cart);
        }

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

        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(m => m.ProductID == id);
            if (existingItem != null)
                cart.Remove(existingItem);

            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return RedirectToAction("ShowCart");
        }

        public IActionResult ClearCart()
        {
            ApplicationContext.SetSessionData(SHOPPING_CART, new List<CartItem>());
            return RedirectToAction("ShowCart");
        }

        public IActionResult ShowCart()
        {
            var cart = GetCart();
            return View(cart);
        }

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
                EmployeeID = 1 // Tạm thời
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
