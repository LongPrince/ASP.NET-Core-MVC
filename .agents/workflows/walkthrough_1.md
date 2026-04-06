---
description: BÁO CÁO TIẾN ĐỘ HOÀN THÀNH
---

# Báo cáo hoàn thành: Giai đoạn 1 Shop App (Đăng ký, Đăng nhập, Hồ sơ)

## 🎯 Mục tiêu hoàn thành
Đã triển khai thành công 3 chức năng đầu tiên của hệ thống `SV22T1020659.Shop` dành cho Khách hàng:
1. Đăng ký tài khoản mới.
2. Đăng nhập vào hệ thống.
3. Quản lý thông tin cá nhân và mật khẩu.

## 🛠️ Các thay đổi chính đã thực hiện
**Giao diện (Frontend)**
- Cài đặt `_Layout.cshtml` mới sử dụng **Bootstrap 5** thay cho cấu trúc AdminLTE, tạo luồng gió mới, thanh thoát, phù hợp với trải nghiệm mua sắm E-commerce.
- Cấu hình file `wwwroot/css/styles.css` cung cấp các Utility Class, Box Shadow hiện đại, bo góc mềm mại.

**Bảo mật & Cấu trúc (Backend)**
- Cấu hình độc lập Session và Cookie Authentication (với scope `SV22T1020659.CustomerCookie`), đảm bảo sự cách ly phiên với project Admin.
- **Mã hóa MD5:** Triển khai cơ chế bảo mật mật khẩu bằng thuật toán MD5 đồng bộ với hệ thống Admin.
- **Tổ chức AppCodes:** Tạo thư mục `AppCodes` trong project Shop và lớp `CryptHelper.cs` để xử lý các hàm tiện ích mã hóa dùng chung.
- Vá lỗi tham chiếu cho SQL Dapper: Đã khai báo thuộc tính `Password` vào model base `Customer` (`SV22T1020659.Models.Partner.Customer`) để khớp câu lệnh `INSERT` của `CustomerRepository.cs`.

**Controllers & Views (Tính năng tối ưu)**
- `AccountController.cs` với logic `AuthenticateAsync` và `ChangePasswordAsync` đã được cập nhật để so sánh mật khẩu mã hóa MD5.
- **Form Đăng ký tối ưu:** Điều chỉnh thứ tự ô nhập (Mật khẩu và Xác nhận mật khẩu nằm cạnh nhau). Tích hợp chọn **Tỉnh/Thành** qua Dropdown (Select) và ô nhập **Địa chỉ** để khớp Backend.
- **Trang Profile thông minh:** Chuyển ô nhập Province sang Dropdown Select, tự động nạp dữ liệu từ `DictionaryDataService` giúp người dùng chọn nhanh thay vì nhập tay.
- **Tiện ích Đăng xuất:** Bổ sung nút Đăng xuất trực quan tại Sidebar của trang cá nhân (Profile) và trang Đổi mật khẩu.
- 4 View tương ứng (`Register.cshtml`, `Login.cshtml`, `Profile.cshtml`, `ChangePassword.cshtml`) được thiết kế thẩm mỹ cao, dạng Card-Auth hiện đại.

---

# Giai đoạn 2: Tìm kiếm & Chi tiết sản phẩm

## 🎯 Mục tiêu hoàn thành
Đã triển khai thành công tính năng cốt lõi cho phép khách hàng duyệt và tìm kiếm sản phẩm:
4. Xem, tìm kiếm danh mục mặt hàng theo loại hàng, tên hàng, khoảng giá.
5. Xem thông tin chi tiết của mặt hàng.

## 🛠️ Các thay đổi chính đã thực hiện

### Backend & Cấu trúc
- Xử lý logic tìm kiếm, phân trang và hiển thị chi tiết (Dữ liệu Controller gọi đến lõi Models chung).
- Cấu hình `PhysicalFileProvider` ánh xạ thư mục ảnh của Admin vào Shop, giải quyết triệt để lỗi không hiển thị hình ảnh sản phẩm.

### Giao diện (Frontend)
- **Trang Danh sách (`Index.cshtml`)**:
    - Sidebar thông minh: Lọc theo Loại hàng và Khoảng giá.
    - **Highlight chủ động**: Tự động làm nổi bật (màu xanh) danh mục hoặc khoảng giá đang được chọn.
    - Grid sản phẩm: Thiết kế dạng Card hiện đại, có hiệu ứng hover và badge trạng thái (Đang bán/Ngừng kinh doanh).
- **Trang Chi tiết (`Detail.cshtml`)**:
    - Thư viện ảnh: Click vào ảnh con để đổi ảnh chính mượt mà qua JavaScript.
    - Thông tin chi tiết: Giá, mô tả, đơn vị tính và bảng thông số kỹ thuật (Attributes).

---

# Giai đoạn 3: Giỏ hàng AJAX (Workflow 6/4)
> [!NOTE]
> Giai đoạn này tập trung vào trải nghiệm người dùng, đảm bảo thao tác mượt mà không tải lại trang.

## 🎯 Mục tiêu hoàn thành
6. Khách hàng thêm sản phẩm vào giỏ.
7. Quản lý, tăng giảm số lượng, xóa sản phẩm khỏi giỏ hàng.

## 🛠️ Các tiến độ kỹ thuật
- Tích hợp hàm `SaveCart`, `GetCart` qua cơ chế Extensions `System.Text.Json` dùng cho Session.
- Xây dựng AJAX thông minh cho các nút bấm:
  - Thêm Toast thông báo "Đã thêm thành công" màu xanh tự tắt.
  - Form xác nhận xóa dùng **Modal Bootstrap** trực quan thay bằng `confirm()` hộp thoại truyền thống.
  - Bộ đếm tự động (Badge số lượng ở Header).

---

# Giai đoạn 4: Đặt mua & VietQR (Workflow 6/4.1)
> [!TIP]
> Ứng dụng tích hợp tự động tạo mã VietQR theo mã ngân hàng chuẩn. Giải quyết được bài toán lọc dữ liệu Order chéo giữa Admin và Shop.

## 🎯 Mục tiêu hoàn thành
8. Đặt hàng qua form xác nhận (Lấy Session ra DB).
9. Màn hình thanh toán chuyển khoản với mã QR động VietQR TCB.
10. Theo dõi lịch sử và chi tiết trạng thái, lộ trình đơn hàng.

## 🛠️ Thay đổi Cốt lõi
- **Nâng cấp Cấu trúc DataLayer chạy song song (Shared)**: Bổ sung `CustomerID` vào `OrderSearchInput.cs` và sửa câu lệnh SQL trong `OrderRepository.cs` để bảo mật dữ liệu User. Ngăn chặn nguy cơ người dùng bên Shop xem được Lịch sử đơn hàng của người khác - một bước vá lỗi cấu trúc hệ thống thiết yếu kế thừa từ ứng dụng Admin cũ.
- **OrderController**: Auth bắt buộc `[Authorize]`. Xử lý tạo Order và OrderDetail đồng bộ từ Giỏ hàng qua `SalesDataService`.
- **Tích hợp VietQR**: Mã Sinh thành công với cú pháp Dynamic. `amount={tong_tien}` và cấu trúc Nội dung đổ ra `<img>` qua máy chủ trung gian `img.vietqr.io`.
- **Giao diện Theo dõi (History & Detail)**: Vẽ Timeline trực quan với Bootstrap cho các bước: Đặt hàng -> Chấp nhận -> Đang giao -> Hoàn thành.

---
*Cập nhật lần cuối: 06/04/2026*