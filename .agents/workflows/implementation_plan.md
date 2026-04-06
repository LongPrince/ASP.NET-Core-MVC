---
description: Kế hoạch triển khai Workflow 6/4.1: Đặt mua hàng & Quản lý Đơn hàng cá nhân
---

# Kế hoạch triển khai Workflow 6/4.1: Đặt mua hàng & Quản lý Đơn hàng cá nhân

Tính năng đặt hàng và tra cứu bắt buộc người dùng tương tác sâu vào Database. Do cấu trúc Backend cũ của Admin thiết kế chưa hỗ trợ trực tiếp việc Lọc đơn hàng theo ID Người dùng, khối lượng công việc sẽ liên quan một phần tới cả tầng Cấu trúc Lõi của hệ thống (BusinessLayers và DataLayers).

## 1. Cập nhật Shared DataLayers (Bắt buộc)
> [!WARNING]
> Tầng Repository hiện tại (`OrderRepository.cs` và `OrderSearchInput.cs`) do được làm cho trang Admin nên đang lấy toàn bộ danh sách đơn hàng. Nếu ứng dụng trang Shop trực tiếp, một khách hàng sẽ nhìn thấy lịch sử mua hàng của toàn bộ những người khác!

- **`OrderSearchInput.cs`**: Bổ sung thêm thuộc tính `public int CustomerID { get; set; } = 0;`.
- **`OrderRepository.cs`**: Cập nhật câu lệnh SQL `ListAsync()` bổ sung mệnh đề `AND (@customerID = 0 OR o.CustomerID = @customerID)` ở 2 cả câu `SELECT` cấu trúc và câu đếm `COUNT(1)`. Thêm tham số `customerID = input.CustomerID` vào tham số gửi bằng Dapper.

## 2. Triển khai OrderController (Shop)
Controller `OrderController` bắt buộc áp dụng `[Authorize]` trên toàn dải class. Gồm 4 màn hình chính:
- **`Checkout()` [GET & POST]**: 
  - (GET) Tạo giao diện cho phép khách hàng sửa lại địa chỉ nhận hàng/tỉnh thành (mặc định lấy từ thông tin Database).
  - (POST) Khi nhấn nút "Xác nhận Đặt Hàng": Khởi tạo Model `Order`, gọi `SalesDataService.AddOrderAsync`. Sau đó, lặp trong Session Giỏ hàng lưu xuống `OrderDetail` gọi `SalesDataService.AddDetailAsync`. Cuối cùng: Gọi Crl `ClearCart` -> Redirect qua trang `Payment`.
- **`Payment(int id)`**:
  - Giao diện thanh toán VietQR ngân hàng TCB theo mô tả của bạn. Sử dụng đường link tự sinh mã QR: 
    `https://img.vietqr.io/image/TCB-111183866868-compact2.png?amount={Total}&addInfo=ID+Order+{OrderID}+Lay+ID+{CustomerID}+Name+{CustomerName}&accountName=DANG+VAN+LONG`
  - Render màn hình với QR Code + Xác nhận thanh toán thành công (Về Lịch sử).
- **`Index()` (Lịch sử Mua hàng)**:
  - Khởi tạo `OrderSearchInput` lấy phân trang, gán `CustomerID = User.GetId()` -> Truyền PagedResult ra View tạo bảng.
- **`Details(int id)` / (Trạng thái đơn hàng)**:
  - Lấy thông tin `SalesDataService.GetOrderAsync(id)` và list `SalesDataService.ListDetailsAsync(id)`. View tạo Timeline (Dựa theo `Status` New/Accept/Shipping...) hoặc giao diện trực quan thân thiện.

## 3. Cập nhật View và Menu
- Màn hình `Cart/Index.cshtml`: Nút "Thanh toán" sẽ trỏ href sang `Order/Checkout`.
- Màn hình Navbar: Menu thả xuống của User Profile thêm link truy cập "Lịch sử mua hàng".

---
Bạn có đồng ý với logic triển khai (đặc biệt là việc nâng cấp cấu trúc Filter CustomerID vào DataLayers) không? Nếu đồng ý, xin phản hồi để tôi bắt đầu code.
