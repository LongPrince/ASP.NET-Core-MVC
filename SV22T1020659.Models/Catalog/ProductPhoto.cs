namespace SV22T1020659.Models.Catalog
{
    /// <summary>
    /// Ảnh của mặt hàng
    /// </summary>
    public class ProductPhoto
    {
        /// <summary>
        /// Mã ảnh
        /// </summary>
        public long PhotoID { get; set; }
        /// <summary>
        /// Mã mặt hàng
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// Tên file ảnh
        /// </summary>
        public string Photo { get; set; } = string.Empty;
        /// <summary>
        /// Mô tả ảnh
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Thứ tự hiển thị (giá trị nhỏ sẽ hiển thị trước)
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Có ẩn ảnh đối với khách hàng hay không?
        /// </summary>
        public bool IsHidden { get; set; }
    }
}
