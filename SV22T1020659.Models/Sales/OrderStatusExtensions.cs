namespace SV22T1020659.Models.Sales
{
    /// <summary>
    /// Mở rộng các phương thức cho enum OrderStatusEnum
    /// </summary>
    public static class OrderStatusExtensions
    {
        /// <summary>
        /// Lấy chuỗi mô tả cho từng trạng thái của đơn hàng
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetDescription(this OrderStatusEnum status)
        {
            return status switch
            {
                OrderStatusEnum.Rejected => "Đơn hàng bị từ chối",
                OrderStatusEnum.Cancelled => "Đơn hàng đã bị hủy",
                OrderStatusEnum.New => "Đơn hàng vừa tạo",
                OrderStatusEnum.Accepted => "Đơn hàng đã được duyệt",
                OrderStatusEnum.Shipping => "Đơn hàng đang được vận chuyển",
                OrderStatusEnum.Completed => "Đơn hàng đã hoàn tất",
                _ => "Không xác định"
            };
        }
    }
}
