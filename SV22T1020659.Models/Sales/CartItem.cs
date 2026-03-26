namespace SV22T1020659.Models.Sales
{
    /// <summary>
    /// Bản ghi mặt hàng được chọn trong giỏ hàng
    /// </summary>
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Unit { get; set; } = "";
        public string Photo { get; set; } = "";
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice 
        { 
            get 
            {
                return Quantity * SalePrice;
            }
        }
    }
}
