using SV22T1020659.Models.Common;

namespace SV22T1020659.Models.Sales
{
    /// <summary>
    /// Đầu vào tìm kiếm, phân trang đơn hàng
    /// </summary>
    public class OrderSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        public OrderStatusEnum Status { get; set; }
        /// <summary>
        /// Từ ngày (ngày lập đơn hàng)
        /// </summary>
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// Đến ngày (ngày lập đơn hàng)
        /// </summary>
        public DateTime? DateTo { get; set; }
        /// <summary>
        /// ID Khách hàng (dùng để lọc riêng biệt theo user, 0 nghĩa là hiển thị tất cả)
        /// </summary>
        public int CustomerID { get; set; } = 0;
    }
}
