using Microsoft.AspNetCore.Http;
using System.Text.Json;
using SV22T1020659.Models.Sales;

namespace SV22T1020659.Shop.AppCodes
{
    /// <summary>
    /// Các phương thức mở rộng cho các đối tượng trong hệ thống
    /// </summary>
    public static class WebExtension
    {
        private const string CART_SESSION_KEY = "Cart";

        /// <summary>
        /// Lưu giỏ hàng vào Session
        /// </summary>
        public static void SetCart(this ISession session, List<CartItem> cart)
        {
            session.SetString(CART_SESSION_KEY, JsonSerializer.Serialize(cart));
        }

        /// <summary>
        /// Lấy giỏ hàng từ Session
        /// </summary>
        public static List<CartItem> GetCart(this ISession session)
        {
            string? cartJson = session.GetString(CART_SESSION_KEY);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        /// <summary>
        /// Xóa sạch giỏ hàng trong Session
        /// </summary>
        public static void ClearCart(this ISession session)
        {
            session.Remove(CART_SESSION_KEY);
        }
    }
}
