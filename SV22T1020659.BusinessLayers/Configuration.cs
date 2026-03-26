namespace SV22T1020659.BusinessLayers
{
    /// <summary>
    /// Lớp lưu trữ các thông tin cấu hình sử dụng trong Business Layer
    /// </summary>
    public class Configuration
    {
        private static string _connecttionString = "";
        /// <summary>
        /// Hàm này có chức năng khởi tạo cấu hình cho Business Layer
        /// (Phải được gọi hàm này khi chạy ứng dụng)
        /// </summary>
        /// <param name="connecttionString"></param>
        public static void Initialize(string connecttionString)
        {
            _connecttionString = connecttionString;
        }
        /// <summary>
        /// Lấy chuỗi tham số kết nối đến cơ sở dữ liệu
        /// </summary>
        public static string ConnectionString => _connecttionString;
    }
}
