namespace SV22T1020659.Models.Sales
{
    /// <summary>
    /// Bản ghi mặt hàng được chọn trong giỏ hàng (Shopping Cart Item)
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Mã mặt hàng
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// Tên mặt hàng
        /// </summary>
        public string ProductName { get; set; } = "";
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; } = "";
        /// <summary>
        /// Tên file ảnh đại diện
        /// </summary>
        public string Photo { get; set; } = "";
        /// <summary>
        /// Số lượng mua
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Giá bán ứng với từng mặt hàng trong giỏ hàng
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// Thành tiền của mặt hàng trong giỏ hàng
        /// </summary>
        public decimal TotalPrice 
        { 
            get 
            {
                return Quantity * SalePrice;
            }
        }
    }
}
