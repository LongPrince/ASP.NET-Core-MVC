using Microsoft.AspNetCore.Mvc;

namespace SV22T1020659.Admin.Controllers
{
    public class Order : Controller
    {
        /// <summary>
        /// Nhập đầu vào tìm kiếm và hiển thị kết quả tìm kiếm đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Giao diện các chức năng hỗ trợ cho nghiệp vụ tạo đơn hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }
       /// <summary>
       /// Tìm kiếm đơn hàng
       /// </summary>
       /// <returns></returns>
        public IActionResult Search()
        {
            return View();
        }
        /// <summary>
        /// Hiển thị thông tin chi tiết của một đơn hàng
        /// Và đồng thời điều hướng tới các chức năng xử lý trên đơn(id)
        /// </summary>
        /// <param name="id">Mã đơn hàng cần cập nhật</param>
        /// <returns></returns>
        public IActionResult Detail(int id)
        {
            return View();
        }
        /// <summary>
        /// Cập nhật thông tin (số lượng, giá bán) của một mặt hàng
        /// Trong giỏ hàng,Trong một đơn hàng
        /// </summary>
        /// <param name="id">0:Giỏ hàng,Khác 0: mã đơn hàng cần xử lý</param>
        /// <param name="productId">Mã mặt hàng cần cập nhật</param>
        /// <returns></returns>
        public IActionResult EditCartItem(int id = 0,int productId = 0)
        {
            if(id == 0) 
            { 
            //Xử lý cho Giỏ Hàng
            }
            else
            {
                //Xử lý cho Đơn Hàng
            }
            return View();
        }
        public IActionResult DeleteCartItem(int id = 0,int productId = 0)
        {
            return View();
        }
        public IActionResult ClearCart()
        {
            return View();
        }
        public IActionResult Accept(int id = 0)
        {
            return View();
        }
        public IActionResult Shipping(int id)
        {
            return View();
        }
        public IActionResult Finish(int id)
        {
            return View();
        }
        public IActionResult Reject(int id)
        {
            return View();
        }
        public IActionResult Cancel(int id)
        {
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
